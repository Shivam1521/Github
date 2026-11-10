using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SmartScanAPI.Controllers
{
    [Authorize(Policy = "ITOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ITOController(ILoggerManager _logger, AuthService authService, IWrapperRepository repo, AppSecrets secrets, EnvironmentVariables variables) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);
        private readonly string URL = variables.Name.Equals("dev", StringComparison.CurrentCultureIgnoreCase)
                || variables.Name.Equals("development", StringComparison.CurrentCultureIgnoreCase) ?
                "https://smartscan.holostikapp.com/" :
                variables.Details.QRURL;

        [AllowAnonymous]
        [HttpPost("Token")]
        public async Task<IActionResult> GetToken([FromBody] AuthRequest request)
        {
            try
            {
                var token = await authService.Authenticate(request);
                if (token == null) return Unauthorized();
                return Ok(new { token.Token });
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetToken Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetToken Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        //[AllowAnonymous]
        [HttpPost("IntegrationProcess")]
        public async Task<IActionResult> IntegrationProcess([FromServices] OAuth2TokenService tokenService)
        {
            try
            {
                var token = await tokenService.GetAccessTokenAsync();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("https://external-api.com/data");

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "External API failed.");

                var data = await response.Content.ReadAsStringAsync();
                return Ok(new { tokenUsed = token, externalData = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"OAuth2 integration failed: {ex.Message}");
            }
        }

        /// <summary>
        /// To Get Pickslip Details with QRs based on Pickslip Number
        /// </summary>
        /// <param name="pickslipNumber"></param>
        /// <returns></returns>
        //[OAuth2Authorize]
        [HttpGet("GetPickSlipDetails")]
        public async Task<ActionResult<SingleResponse<PickSlipQRDetails>>> GetPickSlipDetails(string pickslipNumber)
        {
            SingleResponse<PickSlipQRDetails> res = new();
            try
            {
                int srNo = 1;
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var pickSlipDetails = await repo.PickSlip.GetPickSlipDetails(pickslipNumber);
                if (!pickSlipDetails.Any() || (pickSlipDetails.Count() == 1 && pickSlipDetails.First().PickSlipNumber == "0"))
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Pickslip not found or Items are not picked against given Pickslip Number.";
                    return NotFound(res);
                }
                else
                {
                    PickSlipQRDetails pickSlipQRDetails = new()
                    {
                        PickSlipNumber = pickSlipDetails.First().PickSlipNumber,
                        SourceBU = pickSlipDetails.First().SourceBU,
                        DestinationBU = pickSlipDetails.First().DestinationBU,
                        AddedOn = pickSlipDetails.First().AddedOn
                    };

                    foreach (var pickSlip in pickSlipDetails)
                    {
                        PickSlipItemDetails pickSlipItemDetails = new()
                        {
                            ItemCode = pickSlip.ItemCode,
                            Quantity = pickSlip.Quantity,
                            DeliveryDtlId = pickSlip.DeliveryDtlId
                        };

                        pickSlip.QRs = JsonConvert.DeserializeObject<List<PickSlipQRs>>(pickSlip.SeqNos) ?? [];
                        foreach (var qr in pickSlip.QRs)
                        {
                            if (qr.QRType == PassPhrases.TertiaryPacking)
                            {
                                qr.QR = qrGenerator.GenerateTertiaryQR(qr.SeqNo, qr.SrPrefix);
                            }
                            else if (qr.QRType == PassPhrases.SecondaryPacking)
                            {
                                qr.QR = qrGenerator.GenerateSecondaryQR(qr.SeqNo, qr.SrPrefix);
                            }
                            else if (qr.QRType == PassPhrases.PrePrintedTertiary)
                            {
                                qr.QR = qrGenerator.GeneratePrePrintedTertiaryQR(qr.SeqNo, qr.SrPrefix);
                            }
                            else if (qr.QRType == PassPhrases.PrePrintedSecondary)
                            {
                                qr.QR = qrGenerator.GeneratePrePrintedSecondaryQR(qr.SeqNo, qr.SrPrefix);
                            }
                            else
                            {
                                qr.QR = "Invalid QR Type";
                            }
                            qr.SrNo = srNo;
                            srNo++;
                        }
                        pickSlipItemDetails.QRs = pickSlip.QRs;
                        pickSlipQRDetails.Items.Add(pickSlipItemDetails);
                    }

                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = pickSlipQRDetails;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPickSlipDetails Request: " + pickslipNumber);
                _logger.LogError("GetPickSlipDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// Updates receive status for multiple cartons and returns individual QR results.
        /// </summary>
        [HttpPost("UpdateReceiveStatus")]
        public async Task<ActionResult<HttpResponse<ReceiveCartonResult>>> UpdateReceiveStatus([FromBody] ReceiveCartonsRequest request)
        {
            var response = new HttpResponse<ReceiveCartonResult>();
            var qrResults = new List<ReceiveCartonResult>();

            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);

                if (request?.QRs == null || request.QRs.Count == 0)
                {
                    return BadRequest(new Response
                    {
                        ResponseCode = "400",
                        ResponseMessage = "No QR data provided."
                    });
                }

                //--------------------------------------------------
                // Step 1: Prepare DataTable for TVP
                //--------------------------------------------------
                var table = new DataTable();
                table.Columns.Add("RowId", typeof(int));
                table.Columns.Add("SeqNo", typeof(long));
                table.Columns.Add("Prefix", typeof(string));
                table.Columns.Add("QRType", typeof(string));
                table.Columns.Add("ReceivingLocation", typeof(string));

                int rowId = 1;

                foreach (var qrItem in request.QRs)
                {
                    var result = new ReceiveCartonResult
                    {
                        QR = qrItem.QR,
                        RowId = rowId
                    };

                    try
                    {
                        var carton = ParseQRToReceiveCarton(qrItem.QR, request.ReceivingLocation, userId);

                        if (carton.SeqNo <= 0)
                        {
                            result.StatusCode = "400";
                            result.StatusMessage = "Invalid or unreadable QR.";
                        }
                        else
                        {
                            table.Rows.Add(
                                rowId,
                                carton.SeqNo,
                                carton.Prefix,
                                carton.QRType,
                                carton.ReceivingLocation
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"QR Parse Error: {qrItem.QR} | {ex}");
                        result.StatusCode = "500";
                        result.StatusMessage = "QR parsing failed.";
                    }

                    qrResults.Add(result);
                    rowId++;
                }

                //--------------------------------------------------
                // Step 2: Call BULK SP
                //--------------------------------------------------
                if (table.Rows.Count > 0)
                {
                    var dbResults = await repo.CartonLedger.UpdateReceiveStatusBulk(table, userId);

                    //--------------------------------------------------
                    // Step 3: Map DB response to QR
                    //--------------------------------------------------
                    foreach (var dbRow in dbResults)
                    {
                        var match = qrResults.FirstOrDefault(x => x.RowId == dbRow.RowId);
                        if (match != null)
                        {
                            match.StatusCode = dbRow.StatusCode.ToString();
                            match.StatusMessage = dbRow.StatusMessage;
                        }
                    }
                }

                //--------------------------------------------------
                // Step 4: Summary
                //--------------------------------------------------
                int successCount = qrResults.Count(x => x.StatusCode == "200");
                int failCount = qrResults.Count - successCount;

                response.ResponseCode = successCount > 0 ? "200" : "400";
                response.ResponseMessage = $"Processed {qrResults.Count} QRs: {successCount} succeeded, {failCount} failed.";
                response.ResponseData = qrResults;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateReceiveStatus Exception: {JsonConvert.SerializeObject(ex)} | Request: {JsonConvert.SerializeObject(request)}");
                return StatusCode(500, new Response
                {
                    ResponseCode = "500",
                    ResponseMessage = "Internal server error occurred while updating receive status."
                });
            }
        }

        /// <summary>
        /// Updates dispatch status for multiple cartons and returns individual QR results.
        /// </summary>
        [HttpPost("UpdateDisptachStatus")]
        public async Task<ActionResult<HttpResponse<DispatchCartonResult>>> UpdateDisptachStatus([FromBody] DispatchCartonsRequest request)
        {
            var response = new HttpResponse<DispatchCartonResult>();
            var qrResults = new List<DispatchCartonResult>();

            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);

                if (request?.QRs == null || request.QRs.Count == 0)
                {
                    return BadRequest(new Response
                    {
                        ResponseCode = "400",
                        ResponseMessage = "No QR data provided."
                    });
                }

                var fullCarton = request.QRs.Where(x => x.PickType == "Full");
                var outerPicking = request.QRs.Where(x => x.PickType == "Outer Picking");

                List<DispatchCartonTVP> dispatches = [];


                foreach (var (item, index) in fullCarton.Select((value, i) => (value, i)))
                {
                    var carton = ParseQRDispatchCartonTVP(item.QR, index + 1);
                    dispatches.Add(carton);
                }

                var fullCartonResponse = await repo.CartonLedger.DispatchFullCartons(dispatches, request, userId);

                qrResults.AddRange(fullCartonResponse);

                foreach (var qrItem in outerPicking)
                {
                    var result = new DispatchCartonResult { QR = qrItem.QR };
                    try
                    {
                        var carton = ParseQRDispatchCarton(qrItem.QR, request.PickSlipNumber, userId, qrItem);

                        if (carton.SeqNo <= 0)
                        {
                            result.StatusCode = "400";
                            result.StatusMessage = "Invalid or unreadable QR.";
                            qrResults.Add(result);
                            continue;
                        }

                        var dbResponse = await repo.CartonLedger.UpdateDispatchStatus(carton);
                        result.StatusCode = dbResponse.ResponseCode;
                        result.StatusMessage = dbResponse.ResponseMessage;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing QR: {qrItem.QR} | {JsonConvert.SerializeObject(ex)}");
                        result.StatusCode = "500";
                        result.StatusMessage = "Unexpected error during processing.";
                    }

                    qrResults.Add(result);
                }

                // Summary
                int successCount = qrResults.Count(r => r.StatusCode == "200");
                int failCount = qrResults.Count - successCount;

                response.ResponseCode = successCount > 0 ? "200" : "400";
                response.ResponseMessage = $"Processed {qrResults.Count} QRs: {successCount} succeeded, {failCount} failed.";
                response.ResponseData = qrResults; // include detailed results

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateReceiveStatus Exception: {JsonConvert.SerializeObject(ex)} | Request: {JsonConvert.SerializeObject(request)}");
                return StatusCode(500, new Response
                {
                    ResponseCode = "500",
                    ResponseMessage = "Internal server error occurred while updating receive status."
                });
            }
        }

        [HttpGet("GetOuterQRs")]
        public async Task<ActionResult<SingleResponse<OuterQRsResult>>> GetOuterQRs(string qr)
        {
            try
            {
                SingleResponse<OuterQRsResult> res = new();
                var parsedQR = CommonMethods.ParseQR(qr, _logger, qrGenerator, "GetOuterQRs");
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    var outerQRs = await repo.QR.GetOuterQRs(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType);
                    if (outerQRs != null)
                    {
                        if (outerQRs.First().ResponseCode == "200")
                        {
                            foreach (var item in outerQRs)
                            {
                                var qrData = item.QRType switch
                                {
                                    PassPhrases.PrePrintedSecondary => qrGenerator.GeneratePrePrintedSecondaryQR(item.SeqNo, item.Prefix),
                                    PassPhrases.PrePrintedTertiary => qrGenerator.GeneratePrePrintedTertiaryQR(item.SeqNo, item.Prefix),
                                    PassPhrases.SecondaryPacking => qrGenerator.GenerateSecondaryQR(item.SeqNo, item.Prefix),
                                    PassPhrases.TertiaryPacking => qrGenerator.GenerateTertiaryQR(item.SeqNo, item.Prefix),
                                    _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                                };
                                item.OuterQR = qrData;
                            }
                            res.ResponseCode = outerQRs.First().ResponseCode;
                            res.ResponseMessage = outerQRs.First().ResponseMessage;
                            res.ResponseData = new()
                            {
                                NoOfOuters = outerQRs.Count(),
                                TotalQty = outerQRs.Sum(a => a.Qty),
                                Outers = outerQRs.Any() ? [.. outerQRs] : []
                            };
                            return Ok(res);
                        }
                        else
                        {
                            res.ResponseCode = outerQRs.First().ResponseCode;
                            res.ResponseMessage = outerQRs.First().ResponseMessage;
                            return NotFound(res);
                        }
                    }
                    else
                    {
                        res.ResponseCode = "404";
                        res.ResponseMessage = "No Outer QRs found for the provided Master QR.";
                        return NotFound(res);
                    }
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid QR Code.";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetOuterQRs Request: " + qr);
                _logger.LogError("GetOuterQRs Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        [HttpPost("GetQRDetails")]
        public async Task<ActionResult<HttpResponse<GetQRDetailsResponse>>> GetQRDetails(GetQRDetailsRequest request)
        {
            try
            {
                HttpResponse<GetQRDetailsResponse> res = new();
                List<GetQRDetailsResponse> qrDetails = [];

                foreach (var qrItem in request.QRs)
                {
                    var parsedQR = CommonMethods.ParseQR(qrItem, _logger, qrGenerator, "GetQRDetails");
                    GetQRDetailsResponse qrDetail = new()
                    {
                        QR = qrItem,
                        Status = parsedQR.ParseStatus,
                        ParseStatus = false
                    };
                    if (parsedQR != null && parsedQR.SeqNo != -1)
                    {
                        var qrInfo = await repo.QR.GetQRInfo(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType);
                        if (qrInfo != null && qrInfo.ResponseCode == "200")
                        {
                            qrDetail = qrInfo;
                            qrDetail.QR = qrItem;
                            qrDetail.ParseStatus = true;
                            qrDetail.Status = "Valid QR Code.";
                        }
                        else
                        {
                            qrDetail.Status = "QR Details not found.";
                        }
                    }
                    else
                    {
                        qrDetail.Status = "Invalid QR Code.";
                    }
                    qrDetails.Add(qrDetail);
                }

                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.ResponseData = qrDetails;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetQRDetails Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetQRDetails Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        /// <summary>
        /// Parses and decrypts a QR string into a ReceiveCarton object.
        /// </summary>
        private ReceiveCarton ParseQRToReceiveCarton(string qrData, string receivingLocation, int performedBy)
        {
            ReceiveCarton carton = new()
            {
                SeqNo = -1, // default invalid
                Prefix = string.Empty,
                QRType = "U",
                ReceivingLocation = receivingLocation,
                PerformedBy = performedBy
            };

            if (string.IsNullOrWhiteSpace(qrData))
            {
                _logger.LogError("QRData is null or empty.");
                return carton;
            }

            try
            {
                // Fix encoding issue
                qrData = qrData.Replace(":%5D", ":]");

                string encryptedSegment;

                // Handle both QR formats
                if (qrData.Contains("pa?uid=:]C1", StringComparison.OrdinalIgnoreCase))
                {
                    // Format F1 (URL style)
                    var parts = qrData.Split("pa?uid=:]C1", StringSplitOptions.None);
                    if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _logger.LogError($"Invalid QR format: {qrData}");
                        return carton;
                    }

                    encryptedSegment = parts[1].Replace(":", "").Trim();
                }
                else
                {
                    // Format F2 (raw QR code)
                    encryptedSegment = qrData.Replace(":", "").Trim();
                }

                if (string.IsNullOrWhiteSpace(encryptedSegment))
                {
                    _logger.LogError("Invalid QR format: empty encrypted data.");
                    return carton;
                }

                var decrypted = qrGenerator.Decrypt(encryptedSegment);

                if (string.IsNullOrWhiteSpace(decrypted) || decrypted.Length < 4)
                {
                    _logger.LogError($"Invalid decrypted data for QR: {qrData}");
                    return carton;
                }

                // Extract prefix and sequence
                char qrPrefixChar = decrypted[0];
                carton.Prefix = decrypted.Substring(1, 2);

                if (!long.TryParse(decrypted[3..], out long seqNo))
                {
                    _logger.LogError($"Failed to parse sequence number in QR: {qrData}");
                    return carton;
                }

                carton.SeqNo = seqNo;

                // Map QR type
                carton.QRType = qrPrefixChar switch
                {
                    var c when c.ToString() == PassPhrases.SecondaryPacking => PassPhrases.SecondaryPacking,
                    var c when c.ToString() == PassPhrases.TertiaryPacking => PassPhrases.TertiaryPacking,
                    var c when c.ToString() == PassPhrases.PrePrintedSecondary => PassPhrases.PrePrintedSecondary,
                    var c when c.ToString() == PassPhrases.PrePrintedTertiary => PassPhrases.PrePrintedTertiary,
                    _ => "U" // Unknown
                };
            }
            catch (CryptographicException ex)
            {
                _logger.LogError($"Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }

            return carton;
        }

        /// <summary>
        /// Parses and decrypts a QR string into a ReceiveCarton object.
        /// </summary>
        private DispatchCarton ParseQRDispatchCarton(string qrData, string pickSlipNumber, int performedBy, DispatchQRCodeDetail detail)
        {
            DispatchCarton carton = new()
            {
                SeqNo = -1, // default invalid
                Prefix = string.Empty,
                QRType = "U",
                PickSlipNumber = pickSlipNumber,
                PickType = detail.PickType,
                //FromLocation = fromLocation,
                //ToLocation = toLocation,
                PerformedBy = performedBy,
                Outers = detail.Outers,
            };

            if (string.IsNullOrWhiteSpace(qrData))
            {
                _logger.LogError("QRData is null or empty.");
                return carton;
            }

            try
            {
                // Fix encoding issue
                qrData = qrData.Replace(":%5D", ":]");

                string encryptedSegment;

                // Handle both QR formats
                if (qrData.Contains("pa?uid=:]C1", StringComparison.OrdinalIgnoreCase))
                {
                    // Format F1 (URL style)
                    var parts = qrData.Split("pa?uid=:]C1", StringSplitOptions.None);
                    if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _logger.LogError($"Invalid QR format: {qrData}");
                        return carton;
                    }

                    encryptedSegment = parts[1].Replace(":", "").Trim();
                }
                else
                {
                    // Format F2 (raw QR code)
                    encryptedSegment = qrData.Replace(":", "").Trim();
                }

                if (string.IsNullOrWhiteSpace(encryptedSegment))
                {
                    _logger.LogError("Invalid QR format: empty encrypted data.");
                    return carton;
                }

                var decrypted = qrGenerator.Decrypt(encryptedSegment);

                if (string.IsNullOrWhiteSpace(decrypted) || decrypted.Length < 4)
                {
                    _logger.LogError($"Invalid decrypted data for QR: {qrData}");
                    return carton;
                }

                // Extract prefix and sequence
                char qrPrefixChar = decrypted[0];
                carton.Prefix = decrypted.Substring(1, 2);

                if (!long.TryParse(decrypted[3..], out long seqNo))
                {
                    _logger.LogError($"Failed to parse sequence number in QR: {qrData}");
                    return carton;
                }

                carton.SeqNo = seqNo;

                // Map QR type
                carton.QRType = qrPrefixChar switch
                {
                    var c when c.ToString() == PassPhrases.SecondaryPacking => PassPhrases.SecondaryPacking,
                    var c when c.ToString() == PassPhrases.TertiaryPacking => PassPhrases.TertiaryPacking,
                    var c when c.ToString() == PassPhrases.PrePrintedSecondary => PassPhrases.PrePrintedSecondary,
                    var c when c.ToString() == PassPhrases.PrePrintedTertiary => PassPhrases.PrePrintedTertiary,
                    _ => "U" // Unknown
                };
            }
            catch (CryptographicException ex)
            {
                _logger.LogError($"Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }

            if (carton.Outers.Count > 0)
            {
                foreach (var outer in carton.Outers)
                {
                    var parseOuter = ParseQR(outer.QR);
                    outer.SeqNo = parseOuter.SeqNo;
                    outer.Prefix = parseOuter.Prefix;
                    outer.QRType = parseOuter.QRType;
                    outer.StatusMessage = parseOuter.ParseStatus;
                }
            }

            return carton;
        }

        /// <summary>
        /// Parses and decrypts a QR string into a ReceiveCarton object.
        /// </summary>
        private DispatchCartonTVP ParseQRDispatchCartonTVP(string qrData, int rowId)
        {
            DispatchCartonTVP carton = new()
            {
                RowId = rowId,
                SeqNo = -1,
                Prefix = string.Empty,
                QRType = "U",
                QR = qrData
            };

            if (string.IsNullOrWhiteSpace(qrData))
            {
                _logger.LogError("QRData is null or empty.");
                return carton;
            }

            try
            {
                // Fix encoding issue
                qrData = qrData.Replace(":%5D", ":]");

                string encryptedSegment;

                // Handle both QR formats
                if (qrData.Contains("pa?uid=:]C1", StringComparison.OrdinalIgnoreCase))
                {
                    // Format F1 (URL style)
                    var parts = qrData.Split("pa?uid=:]C1", StringSplitOptions.None);
                    if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _logger.LogError($"Invalid QR format: {qrData}");
                        return carton;
                    }

                    encryptedSegment = parts[1].Replace(":", "").Trim();
                }
                else
                {
                    // Format F2 (raw QR code)
                    encryptedSegment = qrData.Replace(":", "").Trim();
                }

                if (string.IsNullOrWhiteSpace(encryptedSegment))
                {
                    _logger.LogError("Invalid QR format: empty encrypted data.");
                    return carton;
                }

                var decrypted = qrGenerator.Decrypt(encryptedSegment);

                if (string.IsNullOrWhiteSpace(decrypted) || decrypted.Length < 4)
                {
                    _logger.LogError($"Invalid decrypted data for QR: {qrData}");
                    return carton;
                }

                // Extract prefix and sequence
                char qrPrefixChar = decrypted[0];
                carton.Prefix = decrypted.Substring(1, 2);

                if (!long.TryParse(decrypted[3..], out long seqNo))
                {
                    _logger.LogError($"Failed to parse sequence number in QR: {qrData}");
                    return carton;
                }

                carton.SeqNo = seqNo;

                // Map QR type
                carton.QRType = qrPrefixChar switch
                {
                    var c when c.ToString() == PassPhrases.SecondaryPacking => PassPhrases.SecondaryPacking,
                    var c when c.ToString() == PassPhrases.TertiaryPacking => PassPhrases.TertiaryPacking,
                    var c when c.ToString() == PassPhrases.PrePrintedSecondary => PassPhrases.PrePrintedSecondary,
                    var c when c.ToString() == PassPhrases.PrePrintedTertiary => PassPhrases.PrePrintedTertiary,
                    _ => "U" // Unknown
                };
            }
            catch (CryptographicException ex)
            {
                _logger.LogError($"Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            return carton;
        }

        private QRParseResult ParseQR(string qrData)
        {
            var result = new QRParseResult();

            try
            {
                if (string.IsNullOrWhiteSpace(qrData))
                {
                    result.ParseStatus = "QR data is null or empty.";
                    return result;
                }

                // Normalize encoding
                qrData = qrData.Replace(":%5D", ":]").Trim();

                string encryptedSegment;

                // Handle both QR formats
                if (qrData.Contains("pa?uid=:]C1", StringComparison.OrdinalIgnoreCase))
                {
                    // Format F1 (URL style)
                    var parts = qrData.Split("pa?uid=:]C1", StringSplitOptions.None);
                    if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        result.ParseStatus = "Invalid QR format: missing encrypted segment.";
                        _logger.LogError($"Invalid QR format: {qrData}");
                        return result;
                    }

                    encryptedSegment = parts[1].Replace(":", "").Trim();
                }
                else
                {
                    // Format F2 (raw QR code)
                    encryptedSegment = qrData.Replace(":", "").Trim();
                }

                if (string.IsNullOrWhiteSpace(encryptedSegment))
                {
                    result.ParseStatus = "Invalid QR format: empty encrypted data.";
                    return result;
                }

                var decrypted = qrGenerator.Decrypt(encryptedSegment);

                if (string.IsNullOrWhiteSpace(decrypted) || decrypted.Length < 4)
                {
                    result.ParseStatus = "Invalid decrypted data.";
                    _logger.LogError($"Invalid decrypted data for QR: {qrData}");
                    return result;
                }

                // Extract prefix and sequence
                char qrPrefixChar = decrypted[0];
                result.Prefix = decrypted.Substring(1, 2);

                if (!long.TryParse(decrypted[3..], out long seqNo))
                {
                    result.ParseStatus = "Failed to parse sequence number.";
                    _logger.LogError($"Failed to parse sequence number in QR: {qrData}");
                    return result;
                }

                result.SeqNo = seqNo;

                // Map QR type
                result.QRType = qrPrefixChar switch
                {
                    var c when c.ToString() == PassPhrases.SecondaryPacking => PassPhrases.SecondaryPacking,
                    var c when c.ToString() == PassPhrases.TertiaryPacking => PassPhrases.TertiaryPacking,
                    var c when c.ToString() == PassPhrases.PrePrintedSecondary => PassPhrases.PrePrintedSecondary,
                    var c when c.ToString() == PassPhrases.PrePrintedTertiary => PassPhrases.PrePrintedTertiary,
                    _ => "U" // Unknown type
                };

                result.ParseStatus = "Success";
            }
            catch (CryptographicException ex)
            {
                result.ParseStatus = "Decryption failed.";
                _logger.LogError($"Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                result.ParseStatus = "Unexpected error during parsing.";
                _logger.LogError($"Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }

            return result;
        }
    }
}

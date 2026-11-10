using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PickslipController(IWrapperRepository repo, ILoggerManager _logger, AppSecrets secrets, EnvironmentVariables variables, IWebHostEnvironment _env, EBSTokenGenerator tokenGenerator, IOptions<EBSConfig> config) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        /// <summary>
        /// To Get PickSlip Data at the time of Pick up of cartons.
        /// </summary>
        /// <param name="pickSlipNumber"></param>
        /// <returns></returns>
        [HttpGet("GetPickSlipDetailsForPickup")]
        public async Task<ActionResult<SingleResponse<PickSlip>>> GetPickSlipDetailsForPickup(string pickSlipNumber)
        {
            SingleResponse<PickSlip> res = new();
            try
            {
                var result = await repo.PickSlip.GetPickSlipDetailsForPickup(pickSlipNumber);
                if (result.PickSlipNumber == "0")
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Pickslip Number or not received from EBS";
                    return NotFound(res);
                }
                else if (result.PickSlipNumber == "-1")
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Scanned Pickslip is already picked";
                    return NotFound(res);
                }
                else
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    if (result.Items != null && result.Items.Length > 0)
                    {
                        var itemDetails = JsonConvert.DeserializeObject<List<PickSlipItem>>(result.Items);
                        if (itemDetails != null)
                        {
                            result.ItemDetails = itemDetails;
                        }
                    }
                    res.ResponseData = result;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPickSlipDetailsForPickup Request: " + pickSlipNumber);
                _logger.LogError("GetPickSlipDetailsForPickup: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet("GetDetailsForReceving")]
        public async Task<ActionResult<SingleResponse<Received>>> GetDetailsForReceving(string documentNumber)
        {
            SingleResponse<Received> res = new();
            try
            {
                var result = await repo.PickSlip.GetDetailsForReceving(documentNumber);
                if (result.documentNumber == "0" || result.documentNumber == "-2" )
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Document Number or not received yet";
                    return NotFound(res);
                }
                else if (result.documentNumber == "-1")
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Receiving Document is already Scanned";
                    return NotFound(res);
                }
                else
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    if (result.Items != null && result.Items.Length > 0)
                    {
                        var itemDetails = JsonConvert.DeserializeObject<List<ReceivedItem>>(result.Items);
                        if (itemDetails != null)
                        {
                            result.ItemDetails = itemDetails;
                        }
                    }
                    res.ResponseData = result;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPickSlipDetailsForPickup Request: " + documentNumber);
                _logger.LogError("GetPickSlipDetailsForPickup: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Verify if QR is eligible for pickup or not.
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpPost("VerifyQRForPickslip")]
        public async Task<ActionResult<SingleResponse<PickupQRDetailsResponse>>> VerifyQRForPickslip(PickupQRDetailsRequest pickRequest)
        {
            SingleResponse<PickupQRDetailsResponse> res = new();
            try
            {
                var parseOuter = CommonMethods.ParseQR(pickRequest.QR, _logger, qrGenerator, "VerifyQRForPickslip");

                if (parseOuter.SeqNo <= 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid or unreadable QR.";
                    return BadRequest(res);
                }
                else
                {
                    PickupQRDetailsRequestDB request = new()
                    {
                        SeqNo = parseOuter.SeqNo,
                        Prefix = parseOuter.Prefix,
                        QRType = parseOuter.QRType,
                        PickSlipNumber = pickRequest.PickSlipNumber,
                        Loose = pickRequest.Loose,
                    };

                    var result = await repo.PickSlip.VerifyQRForPickslip(request);
                    if (result.ResponseCode == "200")
                    {
                        res.ResponseCode = result.ResponseCode;
                        res.ResponseMessage = result.ResponseMessage;
                        res.ResponseData = result;
                        return Ok(res);
                    }
                    else
                    {
                        res.ResponseCode = result.ResponseCode;
                        res.ResponseMessage = result.ResponseMessage;
                        return BadRequest(res);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("VerifyQRForPickslip Request: " + JsonConvert.SerializeObject(pickRequest));
                _logger.LogError("VerifyQRForPickslip: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Bind Outer/Master QR Codes against Pickslip Number
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("InsertPickSlipDetails")]
        public async Task<ActionResult<Response>> InsertPickSlipDetails(InsertPickSlipRequest request)
        {
            Response res = new();
            try
            {
                request.AddedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                if (string.IsNullOrWhiteSpace(request.PickSlipNumber))
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "PickSlipNumber is required.";
                    return BadRequest(res);
                }

                if (request.PickSlipDetails == null || request.PickSlipDetails.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "PickSlipDetails list cannot be empty.";
                    return BadRequest(res);
                }

                var result = await repo.PickSlip.InsertPickSlipDetails(request);
                if (result.ResponseCode == "200")
                {
                    var token = await tokenGenerator.GetToken();
                    HttpClient httpClient = new()
                    {
                        Timeout = Timeout.InfiniteTimeSpan
                    };
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    PickSlipEBSRequest ebsRequest = new();

                    List<PickSlipEBSDetail> pickSlipDetails = [.. request.PickSlipDetails
                        .GroupBy(x => new
                        {
                            x.ItemCode,
                            x.DeliveryDtlId
                        })
                        .Select(g => new PickSlipEBSDetail
                        {
                            pickslip_number = request.PickSlipNumber,
                            item_code = g.Key.ItemCode,
                            DELIVERY_DETAIL_ID = int.TryParse(g.Key.DeliveryDtlId, out var id) ? id : 0,
                            SMART_SCAN_QTY = g.Sum(x => x.Quantity)
                        })];

                    ebsRequest.Pickslip_detail = pickSlipDetails;
                    var url = config.Value.APIEndpoint;
                    EBSAPILog log = new()
                    {
                        APIName = "update-pickslip",
                        URL = url,
                        Request = JsonConvert.SerializeObject(ebsRequest),
                        Response = "",
                        RequestedBy = request.AddedBy
                    };

                    try
                    {
                        var json = JsonConvert.SerializeObject(ebsRequest);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(url, content);
                        log.Response = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        log.Response = JsonConvert.SerializeObject(ex);
                        _logger.LogError("InsertPickSlipDetails - EBS Call Failed: " + JsonConvert.SerializeObject(ex));
                    }

                    _ = await repo.ERP.LogEBSAPIRequestResponse(log);

                    res.ResponseCode = result.ResponseCode;
                    res.ResponseMessage = result.ResponseMessage;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = result.ResponseCode;
                    res.ResponseMessage = result.ResponseMessage;
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("InsertPickSlipDetails Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("InsertPickSlipDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Temporarily Stored QRs against Pickslip.
        /// </summary>
        /// <param name="pickslipNumber"></param>
        /// <returns></returns>
        [HttpGet("GetTempPickslipQRs")]
        public async Task<ActionResult<HttpResponse<PickupQRDetails>>> GetTempPickslipQRs(string pickslipNumber)
        {
            HttpResponse<PickupQRDetails> res = new();
            try
            {
                var result = await repo.PickSlip.GetTempPickslipQRs(pickslipNumber);
                if (result.Any() && result.First().SeqNo != 0)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No data found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetTempPickslipQRs Request: " + pickslipNumber);
                _logger.LogError("GetTempPickslipQRs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet("GetTempRecivedQRs")]
        public async Task<ActionResult<HttpResponse<PickupQRDetails>>> GetTempRecivedQRs(string pickslipNumber)
        {
            HttpResponse<PickupQRDetails> res = new();
            try
            {
                var result = await repo.PickSlip.GetTempRecivedQRs(pickslipNumber);
                if (result.Any() && result.First().SeqNo != 0)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No data found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetTempRecivedQRs Request: " + pickslipNumber);
                _logger.LogError("GetTempRecivedQRs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Picked slips Detailed Report.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetPickedslipReport")]
        public async Task<ActionResult<HttpResponse<PickSlipReportDetails>>> GetPickedslipReport(TableRequest request)
        {
            HttpResponse<PickSlipReportDetails> res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PickSlip.GetPickedslipsDetails(request);
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        var qrData = item.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => qrGenerator.GeneratePrePrintedSecondaryQR(item.SeqNo, item.Prefix),
                            PassPhrases.PrePrintedTertiary => qrGenerator.GeneratePrePrintedTertiaryQR(item.SeqNo, item.Prefix),
                            PassPhrases.SecondaryPacking => qrGenerator.GenerateSecondaryQR(item.SeqNo, item.Prefix),
                            PassPhrases.TertiaryPacking => qrGenerator.GenerateTertiaryQR(item.SeqNo, item.Prefix),
                            _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                        };

                        item.QR = qrData;
                        item.QRType = item.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => "Outer",
                            PassPhrases.PrePrintedTertiary => "Master",
                            PassPhrases.SecondaryPacking => "Outer",
                            PassPhrases.TertiaryPacking => "Master",
                            _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                        };
                    }

                    // In your controller:
                    //string webRootPath = _env.WebRootPath; // Inject IWebHostEnvironment _env
                    //string folderPath = Path.Combine(webRootPath, "PrintedQRs");
                    //Directory.CreateDirectory(folderPath);

                    //string fileName = $"PickedslipsDetails_{DateTime.Now:yyyyMMddHHmmssffff}.csv";
                    //string filePath = Path.Combine(folderPath, fileName);

                    //CommonMethods.ToCsv(result, filePath, ["Prefix"]);

                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No data found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPickedslipReport Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetPickedslipReport: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Status of QRs of a Pickslip Number.
        /// </summary>
        /// <param name="pickslipNumber"></param>
        /// <returns></returns>
        [HttpGet("GetPickSlipReceiptStatus")]
        public async Task<ActionResult<HttpResponse<PickSlipReceiptStatus>>> GetPickSlipReceiptStatus(string pickslipNumber)
        {
            HttpResponse<PickSlipReceiptStatus> res = new();
            try
            {
                var result = await repo.PickSlip.GetPickSlipReceiptStatus(pickslipNumber);
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        var qrData = item.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => qrGenerator.GeneratePrePrintedSecondaryQR(item.SeqNo, item.Prefix),
                            PassPhrases.PrePrintedTertiary => qrGenerator.GeneratePrePrintedTertiaryQR(item.SeqNo, item.Prefix),
                            PassPhrases.SecondaryPacking => qrGenerator.GenerateSecondaryQR(item.SeqNo, item.Prefix),
                            PassPhrases.TertiaryPacking => qrGenerator.GenerateTertiaryQR(item.SeqNo, item.Prefix),
                            _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                        };

                        item.QR = qrData;
                        item.QRType = item.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => "Outer",
                            PassPhrases.PrePrintedTertiary => "Master",
                            PassPhrases.SecondaryPacking => "Outer",
                            PassPhrases.TertiaryPacking => "Master",
                            _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                        };
                    }

                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Pickslip Number";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPickSlipReceiptStatus Request: " + pickslipNumber);
                _logger.LogError("GetPickSlipReceiptStatus: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }


        //[HttpPost("Dispatchverify")]
        //public async Task<ActionResult<HttpResponse<DispatchCartonResult>>> Dispatchverify([FromBody] DispatchCartonsRequest request)
        //{
        //    var response = new HttpResponse<DispatchCartonResult>();
        //    var qrResults = new List<DispatchCartonResult>();

        //    try
        //    {
        //        int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);

        //        if (request?.QRs == null || request.QRs.Count == 0)
        //        {
        //            return BadRequest(new Response
        //            {
        //                ResponseCode = "400",
        //                ResponseMessage = "No QR data provided."
        //            });
        //        }
        //        var fullCarton = request.QRs;
        //       // var fullCarton = request.QRs.Where(x => x.PickType == "Full");
        //       // var outerPicking = request.QRs.Where(x => x.PickType == "Outer Picking");

        //        List <DispatchCartonTVP> dispatches = [];


        //        foreach (var (item, index) in fullCarton.Select((value, i) => (value, i)))
        //        {
        //            var carton = ParseQRDispatchCartonTVP(item.QR, index + 1);
        //            dispatches.Add(carton);
        //        }

        //        var fullCartonResponse = await repo.CartonLedger.Dispatchverify(dispatches, request, userId);

        //        qrResults.AddRange(fullCartonResponse);

        //        //foreach (var qrItem in outerPicking)
        //        //{
        //        //    var result = new DispatchCartonResult { QR = qrItem.QR };
        //        //    try
        //        //    {
        //        //        var carton = ParseQRDispatchCarton(qrItem.QR, request.PickSlipNumber, userId, qrItem);

        //        //        if (carton.SeqNo <= 0)
        //        //        {
        //        //            result.StatusCode = "400";
        //        //            result.StatusMessage = "Invalid or unreadable QR.";
        //        //            qrResults.Add(result);
        //        //            continue;
        //        //        }

        //        //        var dbResponse = await repo.CartonLedger.UpdateDispatchStatus(carton);
        //        //        result.StatusCode = dbResponse.ResponseCode;
        //        //        result.StatusMessage = dbResponse.ResponseMessage;
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        _logger.LogError($"Error processing QR: {qrItem.QR} | {JsonConvert.SerializeObject(ex)}");
        //        //        result.StatusCode = "500";
        //        //        result.StatusMessage = "Unexpected error during processing.";
        //        //    }

        //        //    qrResults.Add(result);
        //        //}

        //        // Summary
        //        int successCount = qrResults.Count(r => r.StatusCode == "200");
        //        int failCount = qrResults.Count - successCount;

        //        response.ResponseCode = successCount > 0 ? "200" : "400";
        //        response.ResponseMessage = $"Processed {qrResults.Count} QRs: {successCount} succeeded, {failCount} failed.";
        //        response.ResponseData = qrResults; // include detailed results

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"UpdateReceiveStatus Exception: {JsonConvert.SerializeObject(ex)} | Request: {JsonConvert.SerializeObject(request)}");
        //        return StatusCode(500, new Response
        //        {
        //            ResponseCode = "500",
        //            ResponseMessage = "Internal server error occurred while updating receive status."
        //        });
        //    }
        //}



        //[HttpPost("SubmitDispatch")]
        //public async Task<ActionResult<Response>> SubmitDispatch([FromBody] SubmitDispatchRequest request)
        //{
        //    try
        //    {
        //        int userId = Convert.ToInt32(
        //            User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value
        //        );

        //        if (string.IsNullOrWhiteSpace(request.PickSlipNumber))
        //        {
        //            return BadRequest(new Response
        //            {
        //                ResponseCode = "400",
        //                ResponseMessage = "PickSlipNumber is required."
        //            });
        //        }

        //        var dbResponse = await repo.CartonLedger
        //            .SubmitDispatchFullCartons(request.PickSlipNumber, userId);

        //        return Ok(new Response
        //        {
        //            ResponseCode = dbResponse.ResponseCode,
        //            ResponseMessage = dbResponse.ResponseMessage
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(
        //            $"SubmitDispatch Exception: {JsonConvert.SerializeObject(ex)}"
        //        );

        //        return StatusCode(500, new Response
        //        {
        //            ResponseCode = "500",
        //            ResponseMessage = "Internal server error occurred."
        //        });
        //    }
        //}


        /// <summary>
        /// Updates receive status for single  cartons .
        /// </summary>
        [HttpPost("ReceiveCarton_In")]
        public async Task<ActionResult<ReceiveCartonResponse>> S_UpdateReceiveStatus( [FromBody] SingleReceiveCartonsRequest request)
        {
            try
            {
                int userId = Convert.ToInt32(
                    User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value
                );

                if (string.IsNullOrWhiteSpace(request.QR))
                {
                    return BadRequest(new Response
                    {
                        ResponseCode = "400",
                        ResponseMessage = "QR is required."
                    });
                }

                var carton = ParseQRToReceiveCarton(
                    request.QR,
                    request.ReceivingLocation,
                    userId
                );

                if (carton.SeqNo <= 0)
                {
                    return BadRequest(new Response
                    {
                        ResponseCode = "400",
                        ResponseMessage = "Invalid QR."
                    });
                }

                var result = await repo.CartonLedger.UpdateReceiveStatus(carton);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"UpdateReceiveStatus Exception: {JsonConvert.SerializeObject(ex)}"
                );

                return StatusCode(500, new Response
                {
                    ResponseCode = "500",
                    ResponseMessage = "Internal server error occurred."
                });
            }
        }



        /// <summary>
        /// Submit scanned receive cartons.
        /// </summary>

        [HttpPost("SubmitReceiveCartons")]
        public async Task<ActionResult<Response>> SubmitReceiveCartons(
            [FromBody] SubmitReceiveCartonsRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PickSlipNumber))
                {
                    return BadRequest(new Response
                    {
                        ResponseCode = "400",
                        ResponseMessage = "PickSlipNumber is required."
                    });
                }

                var result = await repo.CartonLedger.SubmitReceiveCartons(request.PickSlipNumber);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"SubmitReceiveCartons Exception: {JsonConvert.SerializeObject(ex)}"
                );

                return StatusCode(500, new Response
                {
                    ResponseCode = "500",
                    ResponseMessage = "Internal server error occurred."
                });
            }
        }


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

        [HttpPost("DemapPickSlipDetail")]
        public async Task<ActionResult<SingleResponse<DemapPickSlipDetailResponse>>> DemapPickSlipDetail(DemapPickSlipDetailRequest demapRequest)
        {
            SingleResponse<DemapPickSlipDetailResponse> res = new();
            try
            {
                var parseOuter = CommonMethods.ParseQR(demapRequest.QR, _logger, qrGenerator, "DemapPickSlipDetail");
                if (parseOuter.SeqNo <= 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid or unreadable QR.";
                    return BadRequest(res);
                }
                else
                {
                    DemapPickSlipDetailRequestDB request = new()
                    {
                        SeqNo = parseOuter.SeqNo,
                        Prefix = parseOuter.Prefix,
                        QRType = parseOuter.QRType,
                    };

                    var result = await repo.PickSlip.DemapPickSlipDetail(request);

                    res.ResponseCode = result.ResponseCode;
                    res.ResponseMessage = result.ResponseMessage;

                    if (result.ResponseCode == "200")
                    {
                        return Ok(res);
                    }
                    else
                    {
                        return BadRequest(res);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("DemapPickSlipDetail Request: " + JsonConvert.SerializeObject(demapRequest));
                _logger.LogError("DemapPickSlipDetail: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet("GetPendingPickslipsForSubmit")]
        public async Task<ActionResult<SingleResponse<List<PendingPickslipResponse>>>> GetPendingPickslipsForSubmit()
        {
            SingleResponse<List<PendingPickslipResponse>> res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);   // <-- NEW

                var result = await repo.PickSlip.GetPendingPickslipsForSubmit(userId);              // <-- CHANGED (added userId param)

                res.ResponseCode = "200";
                res.ResponseMessage = "Pending pickslips fetched successfully.";
                res.ResponseData = result.ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPendingPickslipsForSubmit: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
        [HttpPost("BulkSubmitPickSlips")]
        public async Task<ActionResult<SingleResponse<List<BulkSubmitPickSlipResult>>>> BulkSubmitPickSlips(BulkSubmitPickSlipsRequest submitRequest)
        {
            SingleResponse<List<BulkSubmitPickSlipResult>> res = new();
            try
            {
                if (submitRequest.PickSlipNumbers == null || submitRequest.PickSlipNumbers.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "No PickSlipNumbers provided.";
                    return BadRequest(res);
                }

                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);   // <-- CHANGED (correct claim, matches rest of controller)

                BulkSubmitPickSlipsRequestDB request = new()
                {
                    PickSlipNumbers = submitRequest.PickSlipNumbers,
                    UserId = userId   
                };

                var result = await repo.PickSlip.BulkSubmitPickSlips(request);

                var resultList = result.ToList();

                if (resultList.Any(x => x.ResponseCode != "200"))
                {
                    var error = resultList.First(x => x.ResponseCode != "200");

                    res.ResponseCode = error.ResponseCode;
                    res.ResponseMessage = error.ResponseMessage;
                }
                else
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Batch processed.";
                }

                res.ResponseData = resultList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("BulkSubmitPickSlips Request: " + JsonConvert.SerializeObject(submitRequest));
                _logger.LogError("BulkSubmitPickSlips: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

    }
}

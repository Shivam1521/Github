using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PrintJobController(IWrapperRepository repo, ILoggerManager _logger, AppSecrets secrets, EnvironmentVariables variables) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);
        private readonly string URL = variables.Name.Equals("dev", StringComparison.CurrentCultureIgnoreCase)
                || variables.Name.Equals("development", StringComparison.CurrentCultureIgnoreCase) ?
                "https://smartscan.holostikapp.com/" :
                variables.Details.QRURL;

        /// <summary>
        /// To Create a Print Job against Print Request Data
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPost("CreatePrintJob")]
        public async Task<ActionResult<SingleResponse<PrintJobResponse>>> CreatePrintJob(PrintJobRequest job)
        {
            SingleResponse<PrintJobResponse> res = new();
            try
            {
                job.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PrintJob.CreatePrintJob(job);
                if (result.ResponseCode == "200")
                {
                    res.ResponseCode = result.ResponseCode;
                    res.ResponseMessage = result.ResponseMessage;
                    res.ResponseData = JsonConvert.DeserializeObject<List<PrintJobResponse>>(result.ResponseData)?.First();
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
                _logger.LogInfo("CreatePrintJob Request: " + JsonConvert.SerializeObject(job));
                _logger.LogError("CreatePrintJob: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Details of Print Job based on Job Number
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet("GetJobDetails")]
        public async Task<ActionResult<SingleResponse<PrintJobResponse>>> GetJobDetails(string jobNo)
        {
            SingleResponse<PrintJobDetails> res = new();
            try
            {
                var result = await repo.PrintJob.GetJobDetails(jobNo);
                if (result.JobId == -1)
                {
                    res.ResponseCode = "201";
                    res.ResponseMessage = "Print Job already Completed";
                    return Ok(res);
                }
                else if (result.JobId != 0)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Job No.";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetJobDetails Request: " + jobNo);
                _logger.LogError("GetJobDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get QRs of Print Job based on Job Id
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("GetQRDetails")]
        public async Task<ActionResult<SingleResponse<QR>>> GetQRDetails(int jobId)
        {
            SingleResponse<QR> res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var jobDetails = await repo.PrintJob.GetJobDetails("", jobId);
                if (jobDetails.JobId == 0)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Job No.";
                    return NotFound(res);
                }

                var lastPrintedNumberDetails = await repo.PrintJob.GetLastPrintedNumber(jobId, userId);

                if (lastPrintedNumberDetails == null)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Print Data Found for this Job.";
                    return NotFound(res);
                }
                else if (lastPrintedNumberDetails.MasterSeqNo == -1)
                {
                    res.ResponseCode = "201";
                    res.ResponseMessage = "Job Completed.";
                    return Ok(res);
                }
                else
                {
                    QR qR = new()
                    {
                        ItemCode = lastPrintedNumberDetails.ItemCode,
                        MasterSeqNo = lastPrintedNumberDetails.MasterSeqNo,
                        Prefix = ":]C1",
                        Postfix = ":",
                        QRURL = URL + "pa?uid=",
                        MasterQR = qrGenerator.GenerateTertiaryQR(lastPrintedNumberDetails.MasterSeqNo, lastPrintedNumberDetails.Prefix),
                        MasterExists = lastPrintedNumberDetails.MasterExists,
                        OuterBoxMinWeight = lastPrintedNumberDetails.OuterBoxMinWeight,
                        OuterBoxMaxWeight = lastPrintedNumberDetails.OuterBoxMaxWeight,
                        MasterQRType= "T",
                        MasterPrefix = lastPrintedNumberDetails.Prefix ,
                        OuterQRs = []
                    };

                    var outerQRs = string.IsNullOrWhiteSpace(lastPrintedNumberDetails.OuterQRDetails)
                        ? []
                        : JsonConvert.DeserializeObject<List<OuterQR>>(lastPrintedNumberDetails.OuterQRDetails) ?? [];

                    foreach (var item in outerQRs)
                    {
                        qR.OuterQRs.Add(new OuterQRDetails
                        {
                            SeqNo = item.SeqNo,
                            OuterQRData = qrGenerator.GenerateSecondaryQR(item.SeqNo, lastPrintedNumberDetails.Prefix),
                            QRType = "S",
                            QRPrefix = lastPrintedNumberDetails.Prefix,
                        });
                    }
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = qR;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetQRDetails Request: " + jobId);
                _logger.LogError("GetQRDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get QR Print Status based on Job Id and Type of QR
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetQRPrintStatus")]
        public async Task<ActionResult<Response>> GetQRPrintStatus(PrintJobStatusUpdateRequest request)
        {
            Response res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PrintJob.GetQRPrintStatus(request);
                if (result.ResponseCode == "200")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetQRPrintStatus Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetQRPrintStatus: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Update QR Print Status based on Job Id and Type of QR
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateQRPrintStatus")]
        public async Task<ActionResult<Response>> UpdateQRPrintStatus(PrintJobStatusUpdateRequest request)
        {
            Response res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PrintJob.UpdateQRPrintStatus(request);
                if (result.ResponseCode == "200")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("UpdateQRPrintStatus Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("UpdateQRPrintStatus: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Pending Print Jobs due to some error or missed while printing
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet("GetPendingPrintJobs")]
        public async Task<ActionResult<SingleResponse<PrintJobResponse>>> GetPendingPrintJobs()
        {
            SingleResponse<PrintJobDetails> res = new();
            try
            {
                var userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PrintJob.GetJobDetails();
                if (result.JobId != 0)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Job No.";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPendingPrintJobs Request: ");
                _logger.LogError("GetPendingPrintJobs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Master Details based on Outer Scanning to Print Master QR.
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetMasterDetailFromOuter")]
        public async Task<ActionResult<SingleResponse<MasterReprintResponse>>> GetMasterDetailFromOuter(string qr)
        {
            SingleResponse<MasterReprintResponse> res = new();
            try
            {
                var parseOuter = CommonMethods.ParseQR(qr, _logger, qrGenerator, "GetMasterDetailFromOuter");

                if (parseOuter.SeqNo <= 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid or unreadable QR.";
                    return BadRequest(res);
                }
                else
                {
                    QRDetailsRequest request = new()
                    {
                        SeqNo = parseOuter.SeqNo,
                        Prefix = parseOuter.Prefix,
                        QRType = parseOuter.QRType
                    };

                    var result = await repo.PrintJob.GetMasterDetailFromOuter(request);
                    if (result.ResponseCode == "200")
                    {
                        res.ResponseCode = result.ResponseCode;
                        res.ResponseMessage = result.ResponseMessage;

                        result.masterQR  = result.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => qrGenerator.GeneratePrePrintedSecondaryQR(result.SeqNo, result.Prefix),
                            PassPhrases.PrePrintedTertiary => qrGenerator.GeneratePrePrintedTertiaryQR(result.SeqNo, result.Prefix),
                            PassPhrases.SecondaryPacking => qrGenerator.GenerateSecondaryQR(result.SeqNo, result.Prefix),
                            PassPhrases.TertiaryPacking => qrGenerator.GenerateTertiaryQR(result.SeqNo, result.Prefix),
                            _ => throw new ArgumentException($"Unsupported QRType: {result.QRType}")
                        };

                        result.QR = result.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedSecondaryQR(result.SeqNo, result.Prefix) + ":",
                            PassPhrases.PrePrintedTertiary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedTertiaryQR(result.SeqNo, result.Prefix) + ":",
                            PassPhrases.SecondaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateSecondaryQR(result.SeqNo, request.Prefix) + ":",
                            PassPhrases.TertiaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateTertiaryQR(result.SeqNo, result.Prefix) + ":",
                            _ => throw new ArgumentException($"Unsupported QRType: {result.QRType}")
                        };
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
                _logger.LogInfo("GetMasterDetailFromOuter Request: " + qr);
                _logger.LogError("GetMasterDetailFromOuter: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Master Details based on Outer Scanning to Print Master QR.
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetMasterDetailForReprint")]
        public async Task<ActionResult<SingleResponse<MasterReprintResponse>>> GetMasterDetailForReprint(string qr)
        {
            SingleResponse<MasterReprintResponse> res = new();
            try
            {
                var parsemaster = CommonMethods.ParseSeqQR(qr, _logger, qrGenerator, "GetMasterDetailForReprint");

                if (parsemaster.SeqNo <= 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid or unreadable QR.";
                    return BadRequest(res);
                }
                else
                {
                    QRDetailsRequest request = new()
                    {
                        SeqNo = parsemaster.SeqNo,
                        Prefix = parsemaster.Prefix,
                        QRType = parsemaster.QRType
                    };

                    var result = await repo.PrintJob.GetMasterDetailForReprint(request);
                    if (result.ResponseCode == "200")
                    {
                        res.ResponseCode = result.ResponseCode;
                        res.ResponseMessage = result.ResponseMessage;

                        result.masterQR = result.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => qrGenerator.GeneratePrePrintedSecondaryQR(result.SeqNo, result.Prefix),
                            PassPhrases.PrePrintedTertiary => qrGenerator.GeneratePrePrintedTertiaryQR(result.SeqNo, result.Prefix),
                            PassPhrases.SecondaryPacking => qrGenerator.GenerateSecondaryQR(result.SeqNo, result.Prefix),
                            PassPhrases.TertiaryPacking => qrGenerator.GenerateTertiaryQR(result.SeqNo, result.Prefix),
                            _ => throw new ArgumentException($"Unsupported QRType: {result.QRType}")
                        };

                        result.QR = result.QRType switch
                        {
                            PassPhrases.PrePrintedSecondary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedSecondaryQR(result.SeqNo, result.Prefix) + ":",
                            PassPhrases.PrePrintedTertiary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedTertiaryQR(result.SeqNo, result.Prefix) + ":",
                            PassPhrases.SecondaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateSecondaryQR(result.SeqNo, request.Prefix) + ":",
                            PassPhrases.TertiaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateTertiaryQR(result.SeqNo, result.Prefix) + ":",
                            _ => throw new ArgumentException($"Unsupported QRType: {result.QRType}")
                        };
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
                _logger.LogInfo("GetMasterDetailForReprint Request: " + qr);
                _logger.LogError("GetMasterDetailForReprint: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Details of Outters for Re-Printing /Reporting
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetOuterDetailsforReprint")]

        public async Task<ActionResult<SingleResponse<OuterDataQRsResult>>> GetOuterDetailsforReprint(string qr)
        {
            try
            {
                SingleResponse<OuterDataQRsResult> res = new();
                var parsedQR = CommonMethods.ParseSeqQR(qr, _logger, qrGenerator, "GetOuterDetailsforReprint");
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    var outerQRs = await repo.QR.GetOuterDetailsforReprint(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType);
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
                                var qrDataUrl = item.QRType switch
                                {
                                    PassPhrases.PrePrintedSecondary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedSecondaryQR(item.SeqNo, item.Prefix) + ":",
                                    PassPhrases.PrePrintedTertiary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedTertiaryQR(item.SeqNo, item.Prefix) + ":",
                                    PassPhrases.SecondaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateSecondaryQR(item.SeqNo, item.Prefix) + ":",
                                    PassPhrases.TertiaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateTertiaryQR(item.SeqNo, item.Prefix) + ":",
                                    _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                                };
                                item.OuterQR = qrData;
                                item.OuterQRURL = qrDataUrl;
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
                _logger.LogInfo("GetOuterDetailsforReprint Request: " + qr);
                _logger.LogError("GetOuterDetailsforReprint Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        /// <summary>
        /// To Get Details of Carton for Attach/Detach.
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpPost("GetCartonDetailsForAttachDetach")]
        public async Task<ActionResult<SingleResponse<CartonDetailsForAttachDetach>>> GetCartonDetailsForAttachDetach(CartonDetailsForAttachDetachRequest request)
        {
            SingleResponse<CartonDetailsForAttachDetach> res = new();
            try
            {
                var parseOuter = CommonMethods.ParseQR(request.QR, _logger, qrGenerator, "GetCartonDetailsForAttachDetach");

                if (parseOuter.SeqNo <= 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid or unreadable QR.";
                    return BadRequest(res);
                }
                else
                {
                    request.SeqNo = parseOuter.SeqNo;
                    request.Prefix = parseOuter.Prefix;
                    request.QRType = parseOuter.QRType;
                    var result = await repo.PrintJob.GetCartonDetailsForAttachDetach(request);
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
                _logger.LogInfo("GetCartonDetailsForAttachDetach Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetCartonDetailsForAttachDetach: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Details of Outters for Re-Printing /Reporting
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetAllOuterQRs")]
        public async Task<ActionResult<SingleResponse<OuterDataQRsResult>>> GetAllOuterQRs(string qr)
        {
            try
            {
                SingleResponse<OuterDataQRsResult> res = new();
                var parsedQR = CommonMethods.ParseQR(qr, _logger, qrGenerator, "GetOuterQRs");
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    var outerQRs = await repo.QR.GetAllOuterQRs(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType);
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
                                var qrDataUrl = item.QRType switch
                                {
                                    PassPhrases.PrePrintedSecondary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedSecondaryQR(item.SeqNo, item.Prefix) + ":",
                                    PassPhrases.PrePrintedTertiary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedTertiaryQR(item.SeqNo, item.Prefix) + ":",
                                    PassPhrases.SecondaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateSecondaryQR(item.SeqNo, item.Prefix) + ":",
                                    PassPhrases.TertiaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateTertiaryQR(item.SeqNo, item.Prefix) + ":",
                                    _ => throw new ArgumentException($"Unsupported QRType: {item.QRType}")
                                };
                                item.OuterQR = qrData;
                                item.OuterQRURL = qrDataUrl;
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
                _logger.LogInfo("GetAllOuterQRs Request: " + qr);
                _logger.LogError("GetAllOuterQRsS Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

       

        //[HttpGet("GetSecondaryEncrypted")]
        //public string GetSecondaryEncrypted(long seqNo, string prefix)
        //{
        //    string abc = URL + "pa?uid=:]C1" +
        //                 qrGenerator.GenerateSecondaryQR(seqNo, prefix) + ":";

        //    return abc;
        //}

        //[HttpGet("GetTertiaryEncrypted")]
        //public string GetTertiaryEncrypted(long seqNo, string prefix)
        //{
        //    string abc = URL + "pa?uid=:]C1" +
        //                 qrGenerator.GenerateTertiaryQR(seqNo, prefix) + ":";

        //    return abc;
        //}
    }
}

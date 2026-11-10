using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
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
    public class PreprintedQRsController(IWrapperRepository repo, ILoggerManager _logger, AppSecrets secrets, EnvironmentVariables variables, IWebHostEnvironment _env) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);
        private readonly string URL = variables.Name.Equals("dev", StringComparison.CurrentCultureIgnoreCase)
                || variables.Name.Equals("development", StringComparison.CurrentCultureIgnoreCase) ?
                "https://smartscan.holostikapp.com/" :
                variables.Details.QRURL;

        /// <summary>
        /// To Get Details of Preprinted QR Print Jobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetPreprintedQRJobs")]
        public async Task<ActionResult<HttpResponse<PreprintedQRDataResponse>>> GetPreprintedQRJobs(PreprintedQRDataRequest request)
        {
            HttpResponse<PreprintedQRDataResponse> res = new();
            try
            {
                var result = await repo.PreprintedQRJob.GetPreprintedQRJobs(request);
                if (result.Any())
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Data Found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPreprintedQRJobs Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetPreprintedQRJobs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }


        /// <summary>
        /// To Get Details of Preprinted QR Print Jobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetPreprintedQRJobssupplier")]
        public async Task<ActionResult<HttpResponse<PreprintedsupplierQRDataResponse>>> GetPreprintedQRJobssupplier(PreprintedsupplierQRDataRequest request)
        {
            HttpResponse<PreprintedsupplierQRDataResponse> res = new();

            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PreprintedQRJob.GetPreprintedQRJobssupplier(request);
                if (result.Any())
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Data Found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPreprintedQRJobssupplier Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetPreprintedQRJobssupplier: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }


        /// <summary>
        /// To Create Preprinted QR Print Job
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreatePreprintedQRJob")]
        public async Task<ActionResult<Response>> CreatePreprintedQRJob(PreprintedQRJobRequest request)
        {
            Response res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.PreprintedQRJob.CreatePreprintedQRJob(request);
                if (res.ResponseCode == "200")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("CreatePreprintedQRJob Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreatePreprintedQRJob: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Create Preprinted QR Print Job for supplier
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreatePreprintedQRJob_supplier")]
        public async Task<ActionResult<Response>> CreatePreprintedQRJob_supplier(PreprintedQRJobSupplierRequest request)
        {
            Response res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.PreprintedQRJob.CreatePreprintedQRJob_supplier(request);
                if (res.ResponseCode == "200")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("CreatePreprintedQRJob_supplier Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreatePreprintedQRJob_supplier: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Mapping B/w outter & master
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("CreatePreprintedQRMapping")]
        public async Task<ActionResult<MappingResponse>> CreatePreprintedQRMapping(PreprintedQRMappingRequest request)
        {
            MappingResponse res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);

              //var outerQR = CommonMethods.ParseQR(request.OuterQr, _logger, qrGenerator, "CreatePreprintedQRMapping");
                var outerQR = request.OuterQr;
                var masterQR = CommonMethods.ParseQR(request.MasterQr, _logger, qrGenerator, "CreatePreprintedQRMapping");
                var bladeQR = CommonMethods.ParseQR(request.BladeQr, _logger, qrGenerator, "CreatePreprintedQRMapping");

                PreprintedQRMappingDBRequest dbRequest = new()
                {
                    OuterQRType = "PrePrintedSecondary" ,
                    OuterSrNo = outerQR,
                    Type= request.Type,
                    MasterQRType = masterQR.QRType == "L" ? "PrePrintedTertiary" : masterQR.QRType,
                    MasterPrefix = masterQR.Prefix,
                    MasterSrNo = Convert.ToInt64(masterQR.SeqNo),
                    BladeQRType = bladeQR.QRType == "X" ? "PrePrintedBlade" : bladeQR.QRType,
                    BladePrefix = bladeQR.Prefix,
                    BladeSrNo = Convert.ToInt64(bladeQR.SeqNo),
                    UserId = request.UserId,
                    ItemCode= request.ItemCode
                };

                res = await repo.PreprintedQRJob.CreatePreprintedQRMapping(dbRequest);

                if (res.ResponseCode == "200" || res.ResponseCode == "400")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("CreatePreprintedQRMapping Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreatePreprintedQRMapping: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

//

        /// To Get QR Details of Preprinted QR Job based on Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetPreprintedQRJobDetails")]
        public async Task<ActionResult<SingleResponse<PreprintedQR>>> GetPreprintedQRJobDetails(int id)
        {
            SingleResponse<PreprintedQR> res = new();
            try
            {
                var result = await repo.PreprintedQRJob.GetPreprintedQRJobDetails(id);
                if (result.Id == 0)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Preprinted QR Job No.";
                    return NotFound(res);
                }

                PreprintedQR qR = new()
                {
                    ItemCode = result.ItemCode,
                    IsExistingStock = result.IsExistingStock,
                    Prefix = ":]C1",
                    Postfix = ":",
                    QRURL = URL + "pa?uid=",
                    QRs = [],
                    QRPrefix  = result.UnitPrefix ,
                    QRType = result.Type == "PrePrintedSecondary" ? PassPhrases.PrePrintedSecondary
                     : result.Type == "PrePrintedTertiary" ? PassPhrases.PrePrintedTertiary
                     : result.Type == "PrePrintedBlade" ? PassPhrases.PrePrintedBlade
                     : "C",
                };

                for (long i = result.FromNo; i <= result.ToNo; i++)
                {
                    qR.QRs.Add(new PreprintedQRDetails
                    {
                        QRseqno=i,
                        QRData = result.Type == "PrePrintedSecondary" ? qrGenerator.GeneratePrePrintedSecondaryQR(i, result.UnitPrefix) : qrGenerator.GeneratePrePrintedTertiaryQR(i, result.UnitPrefix)
                    });
                }
                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.ResponseData = qR;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPreprintedQRJobDetails Request: " + id);
                _logger.LogError("GetPreprintedQRJobDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

//
        /// <summary>
        /// To Get QR Details of Preprinted QR Job based on Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetPreprintedQRJobDetails_s")]
        public async Task<ActionResult<SingleResponse<PreprintedQRsq>>> GetPreprintedQRJobDetails_s(int id)
        {
            SingleResponse<PreprintedQRsq> res = new();
            try
            {
                var result = await repo.PreprintedQRJob.GetPreprintedQRJobDetails_s(id);
                if (result.Id == 0)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Preprinted QR Job No.";
                    return NotFound(res);
                }

                PreprintedQRsq qR = new()
                {
                    ItemCode = result.ItemCode,
                    ItemDesc = result.ItemDesc,
                    IsExistingStock = result.IsExistingStock,
                    QRPrefix  = result.UnitPrefix ,
                    QRType = result.Type == "PrePrintedSecondary" ? PassPhrases.PrePrintedSecondary
                     : result.Type == "PrePrintedTertiary" ? PassPhrases.PrePrintedTertiary
                     : result.Type == "PrePrintedBlade" ? PassPhrases.PrePrintedBlade
                     : "C",
                    Prefix = ":]C1",
                    Postfix = ":",
                    QRURL = URL + "pa?uid=",
                    QRs = []
                };

                for (long i = result.FromNo; i <= result.ToNo; i++)
                {
                    qR.QRs.Add(new PreprintedQRDetails
                    {
                        QRseqno=i,
                       // QRData = result.Type == "PrePrintedSecondary" ? qrGenerator.GeneratePrePrintedSecondaryQR(i, result.UnitPrefix) : qrGenerator.GeneratePrePrintedTertiaryQR(i, result.UnitPrefix)
                        QRData = result.Type == "PrePrintedSecondary" ? qrGenerator.GeneratePrePrintedSecondaryQR(i, result.UnitPrefix)
                         : result.Type == "PrePrintedTertiary" ? qrGenerator.GeneratePrePrintedTertiaryQR(i, result.UnitPrefix)
                         : result.Type == "PrePrintedBlade" ? qrGenerator.GeneratePrePrintedBladeQR(i, result.UnitPrefix)
                         : "",
                    });
                }
                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.ResponseData = qR;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPreprintedQRJobDetails Request: " + id);
                _logger.LogError("GetPreprintedQRJobDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }


        /// <summary>
        /// To Get QR Details of Preprinted QR Job based on Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetPreprintedQRJobDetailsSupplier")]
        public async Task<ActionResult<HttpResponse<PreprintedSupplierQRsq>>> GetPreprintedQRJobDetailsSupplier(int id)
        {
            HttpResponse<PreprintedSupplierQRsq> res = new();

            try
            {
                var result = await repo.PreprintedQRJob.GetPreprintedQRJobDetailsSupplier(id);

                // Check if data exists
                if (result == null || !result.Any())
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "Invalid Preprinted QR Job No.";
                    return NotFound(res);
                }

                // First record contains common information
                var first = result.First();

                PreprintedSupplierQRsq qR = new()
                {
                    ItemCode = first.ItemCode,
                    ItemDesc=first.ItemDesc,
                    QRPrefix = first.UnitPrefix,
                    QRType = first.Type == "PrePrintedSecondary" ? "D" : "L",
                    Prefix = ":]C1",
                    Postfix = ":",
                    QRURL = URL + "pa?uid=",
                    QRs = new List<PreprintedSupplierQRDetails>()

                };

                // Add all QR records
                foreach (var item in result)
                {
                    qR.QRs.Add(new PreprintedSupplierQRDetails
                    {
                        QRData = item.Srno,
                        IsPrinted = item.IsPrinted
                    });
                }

                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.ResponseData = new[] { qR };

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPreprintedQRJobDetailsSupplier Request: " + id);
                _logger.LogError("GetPreprintedQRJobDetailsSupplier: " + JsonConvert.SerializeObject(ex));

                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";

                return StatusCode(500, res);
            }
        }



        /// <summary>
        /// To Update Preprinted QR Job Status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("UpdatePreprintedQRJobStatus")]
        public async Task<ActionResult<Response>> UpdatePreprintedQRJobStatus(int id)
        {
            Response res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.PreprintedQRJob.UpdatePreprintedQRJobStatus(id, userId);
                if (res.ResponseCode == "200")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("UpdatePreprintedQRJobStatus Request: " + id);
                _logger.LogError("UpdatePreprintedQRJobStatus: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Update Preprinted QR Job Status of Supplier
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("UpdatePreprintedQRJobStatussupplier")]
        public async Task<ActionResult<Response>> UpdatePreprintedQRJobStatussupplier(int id)
        {
            Response res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.PreprintedQRJob.UpdatePreprintedQRJobStatussupplier(id, userId);
                if (res.ResponseCode == "200")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("UpdatePreprintedQRJobStatussupplier Request: " + id);
                _logger.LogError("UpdatePreprintedQRJobStatussupplier: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get QR List of QRs for Testing Purpose (Internal Use only)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetQRs")]
        public async Task<ActionResult<SingleResponse<PreprintedQR>>> GetQRs(QRRequest request)
        {
            SingleResponse<PreprintedQR> res = new();
            try
            {
                PreprintedQR qR = new()
                {
                    ItemCode = "",
                    IsExistingStock = false,
                    Prefix = ":]C1",
                    Postfix = ":",
                    QRURL = URL + "pa?uid=",
                    QRs = [],
                    QRPrefix = "",
                    QRType = ""
                };

                for (long i = request.FromNo; i <= request.ToNo; i++)
                {
                    var qrData = request.QRType switch
                    {
                        PassPhrases.PrePrintedSecondary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedSecondaryQR(i, request.UnitPrefix) + ":",
                        PassPhrases.PrePrintedTertiary => URL + "pa?uid=:]C1" + qrGenerator.GeneratePrePrintedTertiaryQR(i, request.UnitPrefix) + ":",
                        PassPhrases.SecondaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateSecondaryQR(i, request.UnitPrefix) + ":",
                        PassPhrases.TertiaryPacking => URL + "pa?uid=:]C1" + qrGenerator.GenerateTertiaryQR(i, request.UnitPrefix) + ":",
                        _ => throw new ArgumentException($"Unsupported QRType: {request.QRType}")
                    };

                    qR.QRs.Add(new PreprintedQRDetails
                    {
                        QRData = qrData
                    });

                }
                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.ResponseData = qR;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetQRs Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetQRs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get QR List of QRs for Testing Purpose (Internal Use only)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetCurrentStock")]
        public async Task<ActionResult<Response>> GetCurrentStock()
        {
            Response res = new();
            try
            {
                var printedQRs = await repo.PreprintedQRJob.GetPrintedSeq();

                foreach (var item in printedQRs)
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
                }

                List<PrintedSeqResponse> printedQRsList = [.. printedQRs.Select(p => new PrintedSeqResponse
                {
                    ItemCode = p.ItemCode,
                    QRType = p.QRType switch
                    {
                        PassPhrases.PrePrintedSecondary => "Outer",
                        PassPhrases.PrePrintedTertiary => "Master",
                        PassPhrases.SecondaryPacking => "Outer",
                        PassPhrases.TertiaryPacking => "Master",
                        _ => throw new ArgumentException($"Unsupported QRType: {p.QRType}")
                    },
                    Qty = p.Qty,
                    QR = p.QR,
                    PrintedOn = p.PrintedOn
                })];

                // In your controller:
                string webRootPath = _env.WebRootPath; // Inject IWebHostEnvironment _env
                string folderPath = Path.Combine(webRootPath, "PrintedQRs");
                Directory.CreateDirectory(folderPath);

                string fileName = $"CurrentStock_{DateTime.Now:yyyyMMddHHmmssffff}.csv";
                string filePath = Path.Combine(folderPath, fileName);

                CommonMethods.ToCsv(printedQRsList, filePath);

                res.ResponseCode = "200";
                res.ResponseMessage = $"{Request.Scheme}://{Request.Host}/PrintedQRs/{fileName}";
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetCurrentStock Request: "); //+ JsonConvert.SerializeObject(request))
                _logger.LogError("GetCurrentStock: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
        [HttpPost("GetPreprintedQrDetailsforReprint")]
        public async Task<ActionResult<SingleResponse<PreprintedQRReprintResponse>>> GetPreprintedQrDetailsforReprint([FromBody] PreprintedQRReprintRequest request)
        {
            SingleResponse<PreprintedQRReprintResponse> res = new();

            try
            {
                
                if (request.Srno.Length < 14)
                {
                    char scannedQRType = request.Srno[0];
                    request.Prefix = request.Srno.Substring(1, 2);
                    request.Srno = (long.Parse(request.Srno[3..])).ToString();

                    // Expected QR Type
                    string expectedQRType = request.QRType switch
                    {
                        "PrePrintedIOW" => PassPhrases.PrePrintedSecondary,
                        "PrePrintedTertiary" => PassPhrases.PrePrintedTertiary,
                        "PrePrintedBlade" => PassPhrases.PrePrintedBlade,
                        _ => string.Empty
                    };

                    if (scannedQRType.ToString() != expectedQRType)
                    {
                        res.ResponseCode = "400";
                        res.ResponseMessage = "Selected QR Type does not match the scanned QR.";
                        return BadRequest(res);
                    }
                }

                    request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PreprintedQRJob.GetPreprintedQrDetailsforReprint(request);

                if (result == null)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "QR Details not found.";
                    return NotFound(res);
                }
                result.Prefix =  ":]C1";
            //    result.QRPrefix = result.Prefix;
                result.QRURL = URL + "pa?uid=";
                result.QRType = request.QRType == "PrePrintedIOW"
                    ? PassPhrases.PrePrintedSecondary
                    : request.QRType == "PrePrintedTertiary"
                        ? PassPhrases.PrePrintedTertiary
                        : request.QRType == "PrePrintedBlade"
                            ? PassPhrases.PrePrintedBlade
                            : "C";


                if (request.QRType != "PrePrintedIOW")
                {
                    result.encryptsrno = result.QRType == "L" ? qrGenerator.GeneratePrePrintedTertiaryQR(Convert.ToInt32(request.Srno), result.QRPrefix)
                        : result.QRType == "X" ? qrGenerator.GeneratePrePrintedBladeQR(Convert.ToInt32(request.Srno), result.QRPrefix)
                        : "";
                }

                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.ResponseData = result;

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPreprintedQrDetailsforReprint Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetPreprintedQrDetailsforReprint: " + JsonConvert.SerializeObject(ex));

                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}

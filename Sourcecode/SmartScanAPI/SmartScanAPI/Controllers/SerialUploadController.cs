using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SerialUploadController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        [HttpPost("UploadSerialExcel")]
        public async Task<ActionResult> UploadSerialExcel(IFormFile uploadedFile, [FromForm] string type)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return BadRequest(new ResponseSerial
                {
                    ResponseCode = "400",
                    ResponseMessage = "No file uploaded."
                });
            }

            int successCount = 0;
            int failureCount = 0;
            List<ResponseSerial> rowResponses = []; // 🔹 To store each row response

            try
            {
                using (var stream = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(stream);
                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return BadRequest(new ResponseSerial
                        {
                            ResponseCode = "400",
                            ResponseMessage = "Excel file is empty or invalid."
                        });
                    }

                    var lastRow = worksheet.LastRowUsed();
                    int rowCount = lastRow != null ? lastRow.RowNumber() : 0;
                    int createdby = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);

                    for (int row = 2; row <= rowCount; row++) // Skip header
                    {
                        var itemCode = worksheet.Cell(row, 1).GetString();
                        var qtyString = worksheet.Cell(row, 2).GetString();
                        var lineNumber = worksheet.Cell(row, 3).GetString();

                        var request = new SrUpload
                        {
                            ItemCode = itemCode,
                            QtyToBeDispatched = int.TryParse(qtyString, out var qty) ? qty : 0,
                            CreatedBy = createdby,
                            Type = type,
                            LineNumber = lineNumber
                        };

                        var actionResult = await CreateUploadSerial(request);

                        ResponseSerial? res = null;

                        if (actionResult.Result is ObjectResult objectResult && objectResult.Value is ResponseSerial r1)
                        {
                            res = r1;
                        }
                        else if (actionResult.Value is ResponseSerial r2)
                        {
                            res = r2;
                        }

                        if (res != null)
                        {
                            // Add row number info
                            res.ResponseMessage = $"Row {row - 1}: {res.ResponseMessage}";
                            rowResponses.Add(res);

                            if (res.ResponseCode == "200")
                                successCount++;
                            else
                                failureCount++;
                        }
                        else
                        {
                            rowResponses.Add(new ResponseSerial
                            {
                                ResponseCode = "500",
                                ResponseMessage = $"Row {row}: Unknown error",
                                ItemCode = itemCode,
                                DispatchedQty = int.TryParse(qtyString, out var q) ? q : 0,
                                LineNumber = lineNumber
                            });
                            failureCount++;
                        }
                    }
                }

                // Final combined response
                return Ok(new
                {
                    ResponseCode = "200",
                    ResponseMessage = $"Records uploaded: {successCount}, Failed: {failureCount}",
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    RowResults = rowResponses // 🔹 All row-wise results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("UploadSerialExcel error: " + ex.Message);

                return StatusCode(500, new ResponseSerial
                {
                    ResponseCode = "500",
                    ResponseMessage = "Internal server error."
                });
            }
        }


        [HttpPost("UploadOneSerial")]
        public async Task<ActionResult<ResponseSerial>> CreateUploadSerial(SrUpload request)
        {
            ResponseSerial res = new();
            try
            {
                res = await repo.SrUpload.CreateUploadSerial(request);
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
                _logger.LogInfo("CreateUploadSerial Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUploadSerial: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        [HttpGet("GetItems")]
        public async Task<ActionResult<HttpResponse<ItemDDL>>> GetItems()
        {
            HttpResponse<ItemDDL> res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.Item.GetItems(userId);
                if (result != null && result.Any())
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
                _logger.LogInfo("GetItems Request: ");
                _logger.LogError("GetItems: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
        [HttpGet("GetItemDetails")]
        public async Task<ActionResult<SingleResponse<ItemDetailsResponse>>> GetItemDetails(int itemid)
        {
            SingleResponse<ItemDetailsResponse> res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.Item.GetItemDetails(itemid, userId);
                if (result != null && result.Any())
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    foreach (var item in result)
                    {
                        if (!string.IsNullOrEmpty(item.Line))
                        {
                            try
                            {
                                item.LineDetails = JsonConvert.DeserializeObject<List<LineDetails>>(item.Line) ?? [];
                            }
                            catch (JsonException)
                            {
                                item.LineDetails = [];
                            }
                        }
                        else
                        {
                            item.LineDetails = [];
                        }
                    }
                    res.ResponseData = result.FirstOrDefault();
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
                _logger.LogInfo("GetItemDetails Request: ");
                _logger.LogError("GetItemDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost("GetItemMasterByUserId")]
        public async Task<ActionResult<HttpResponse<ItemMasterResponse>>> GetItemMasterByUserId(TableRequest request)
        {
            HttpResponse<ItemMasterResponse> res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.Item.GetItemMasterByUserId(request);

                if (result != null && result.Any())
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";

                    foreach (var item in result)
                    {
                        if (!string.IsNullOrEmpty(item.Line))
                        {
                            try
                            {
                                item.Lines = JsonConvert.DeserializeObject<List<Line>>(item.Line) ?? [];
                            }
                            catch (JsonException)
                            {
                                item.Lines = [];
                            }
                        }
                        else
                        {
                            item.Lines = [];
                        }

                        if (item.OuterBoxMaxWeight > 0 && item.OuterBoxMinWeight > 0)
                        {
                            item.OuterBoxWeightRange = [item.OuterBoxMinWeight, item.OuterBoxMaxWeight];
                        }
                    }

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
                _logger.LogInfo("GetItemMasterByUserId Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetItemMasterByUserId: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost("UpdateItemMaster")]
        public async Task<ActionResult<Response>> UpdateItemMaster(ItemMasterRequest request)
        {
            Response res = new();
            try
            {
                int createdby = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                if (request.OuterBoxWeightRange.Length > 0)
                {
                    if (request.OuterBoxWeightRange.Length > 2)
                    {
                        res.ResponseCode = "400";
                        res.ResponseMessage = "Outer Box Weight Range should have only two values";
                        return BadRequest(res);
                    }
                    else
                    {
                        request.OuterBoxMinWeight = request.OuterBoxWeightRange[0];
                        request.OuterBoxMaxWeight = request.OuterBoxWeightRange[1];
                    }
                }
                res = await repo.Item.UpdateItemMaster(request, createdby);
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
                _logger.LogInfo("UpdateItemMaster Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("UpdateItemMaster: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }

        }

        [HttpPost("MapItemsAndUsers")]
        public async Task<ActionResult<Response>> MapItemsAndUsers([FromForm] MapItemsAndUsersRequest request)
        {
            List<ItemUserImportRow>? rows = null;
            Response res = new();
            try
            {
                ValidateFile(request.File);

                var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

                if (extension == ".csv")
                {
                    rows = ParseCsv(request.File);
                }
                else if (extension == ".xlsx" || extension == ".xls")
                {
                    rows = ParseExcel(request.File);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid file, please upload a csv or an excel file.";
                    return BadRequest(res);
                }

                var normalizedRows = NormalizeLineNumbers(rows);

                if (normalizedRows.Count > 10000)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Maximum 10,000 records are allowed in a single upload.";
                    return BadRequest(res);
                }

                var tvp = CreateDataTable(normalizedRows);

                await repo.Item.MapItemsAndUsers(tvp);
                res.ResponseCode = "200";
                res.ResponseMessage = "Uploaded Successfully.";
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("MapItemsAndUsers Request: " + JsonConvert.SerializeObject(rows));
                _logger.LogError("MapItemsAndUsers: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadHttpRequestException("Uploaded file is empty");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType.ToLowerInvariant();

            var isCsv =
                extension == ".csv" &&
                (contentType == "text/csv" || contentType == "application/vnd.ms-excel");

            var isExcel =
                (extension == ".xlsx" || extension == ".xls") &&
                contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            if (!isCsv && !isExcel)
            {
                throw new BadHttpRequestException(
                    "Invalid file type. Only CSV or Excel (.xlsx) files are allowed."
                );
            }
        }

        private static List<ItemUserImportRow> ParseCsv(IFormFile file)
        {
            var result = new List<ItemUserImportRow>();

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                result.Add(new ItemUserImportRow
                {
                    ItemCode = csv.GetField("Item Code"),
                    ItemDescription = csv.GetField("Description"),
                    Brand = csv.GetField("Brand"),
                    MfgLocation = csv.GetField("Mfg Location"),
                    Class = csv.GetField("Class"),
                    MasterPackingSize = csv.GetField<int>("Master Box Packing"),
                    OuterPackingSize = csv.GetField<int>("Outer Box Packing"),
                    Supervisor = csv.GetField("Pic 1 ( Line Supervisor)"),
                    Email = csv.GetField("Email (for login)"),
                    LineNo = csv.GetField("Cell/Flow Wrap Number")
                });
            }

            return result;
        }

        private static List<ItemUserImportRow> ParseExcel(IFormFile file)
        {
            var result = new List<ItemUserImportRow>();

            using var stream = file.OpenReadStream();
            using var doc = SpreadsheetDocument.Open(stream, false);

            var workbookPart = doc.WorkbookPart;
            var sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().First();
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

            var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>().ToList();
            if (rows.Count < 2) return result;

            // Map headers → column index
            var headerMap = BuildHeaderMap(rows[0], workbookPart);

            foreach (var row in rows.Skip(1))
            {
                try
                {
                    result.Add(new ItemUserImportRow
                    {
                        ItemCode = GetCell(row, headerMap, "Item Code", workbookPart),
                        ItemDescription = GetCell(row, headerMap, "Description", workbookPart),
                        Brand = GetCell(row, headerMap, "Brand", workbookPart),
                        MfgLocation = GetCell(row, headerMap, "Mfg Location", workbookPart),
                        Class = GetCell(row, headerMap, "Class", workbookPart),
                        MasterPackingSize = int.TryParse(GetCell(row, headerMap, "Master Box Packing", workbookPart)?.ToString(), out var m) ? m : 0,
                        OuterPackingSize = int.TryParse(GetCell(row, headerMap, "Outer Box Packing", workbookPart)?.ToString(), out var o) ? o : 1,
                        Supervisor = GetCell(row, headerMap, "Pic 1 ( Line Supervisor)", workbookPart),
                        Email = GetCell(row, headerMap, "Email (for login)", workbookPart),
                        LineNo = GetCell(row, headerMap, "Cell/Flow Wrap Number", workbookPart),
                        FanType = GetCell(row, headerMap, "Type of Fan", workbookPart),
                        ModelCode = GetCell(row, headerMap, "Model Code", workbookPart),
                        Size = GetCell(row, headerMap, "Size", workbookPart),
                        ColorCode = GetCell(row, headerMap, "Color Code", workbookPart),
                        Labels = int.TryParse(GetCell(row, headerMap, "Labels", workbookPart)?.ToString(), out var l) ? l : 0
                    });
                }
                catch (Exception Ex)
                {

                }

            }

            return result;
        }

        private static Dictionary<string, string> BuildHeaderMap(Row headerRow, WorkbookPart workbookPart)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var cell in headerRow.Elements<Cell>())
            {
                var columnName = GetColumnName(cell.CellReference);
                var headerText = GetCellValue(cell, workbookPart);

                if (!string.IsNullOrWhiteSpace(headerText))
                    map[headerText.Trim()] = columnName;
            }

            return map;
        }

        private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            if (cell == null || cell.CellValue == null)
                return string.Empty;

            var value = cell.CellValue.InnerText;

            if (cell.DataType == null)
                return value;

            // Use .Value property for switch, as CellValues is not a constant type
            switch (cell.DataType.Value)
            {
                case var dt when dt == CellValues.SharedString:
                    return workbookPart.SharedStringTablePart.SharedStringTable
                        .Elements<SharedStringItem>()
                        .ElementAt(int.Parse(value))
                        .InnerText;
                default:
                    return value;
            }
        }

        private static string GetColumnName(string cellReference)
        {
            return new string([.. cellReference.Where(char.IsLetter)]);
        }

        private static string GetCell(
            Row row,
            Dictionary<string, string> headerMap,
            string header,
            WorkbookPart workbookPart)
        {
            if (!headerMap.TryGetValue(header, out var column))
                return string.Empty;

            var cell = row.Elements<Cell>()
                          .FirstOrDefault(c => GetColumnName(c.CellReference) == column);

            return GetCellValue(cell, workbookPart);
        }


        private static void ValidateHeaders(Dictionary<string, string> headers)
        {
            string[] required =
            [
                "Item Code",
                "Mfg Location",
                "Cell/Flow Wrap Number"
            ];

            var missing = required.Where(r => !headers.ContainsKey(r)).ToList();
            if (missing.Count != 0)
                throw new BadHttpRequestException(
                    "Missing required columns: " + string.Join(", ", missing)
                );
        }


        private static List<ItemUserImportRow> NormalizeLineNumbers(List<ItemUserImportRow> rows)
        {
            var result = new List<ItemUserImportRow>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.LineNo))
                {
                    result.Add(row);
                    continue;
                }

                var lines = row.LineNo
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim());

                foreach (var line in lines)
                {
                    result.Add(new ItemUserImportRow
                    {
                        ItemCode = row.ItemCode,
                        ItemDescription = row.ItemDescription,
                        Brand = row.Brand,
                        MfgLocation = row.MfgLocation,
                        Class = row.Class,
                        MasterPackingSize = row.MasterPackingSize,
                        OuterPackingSize = row.OuterPackingSize,
                        Supervisor = row.Supervisor,
                        Email = row.Email,
                        LineNo = line,
                        FanType = row.FanType,
                        ModelCode = row.ModelCode,
                        Size = row.Size,
                        ColorCode = row.ColorCode,
                        Labels = row.Labels
                    });
                }
            }

            return result;
        }

        private static DataTable CreateDataTable(List<ItemUserImportRow> rows)
        {
            var table = new DataTable();
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("ItemDescription", typeof(string));
            table.Columns.Add("Brand", typeof(string));
            table.Columns.Add("Class", typeof(string));
            table.Columns.Add("MfgLocation", typeof(string));
            table.Columns.Add("MasterPackingSize", typeof(int));
            table.Columns.Add("OuterPackingSize", typeof(int));
            table.Columns.Add("Supervisor", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("LineNo", typeof(string));
            table.Columns.Add("FanType", typeof(string));
            table.Columns.Add("ModelCode", typeof(string));
            table.Columns.Add("Size", typeof(string));
            table.Columns.Add("ColorCode", typeof(string));
            table.Columns.Add("Labels", typeof(int));

            foreach (var r in rows)
            {
                table.Rows.Add(
                    r.ItemCode,
                    r.ItemDescription,
                    r.Brand,
                    r.Class,
                    r.MfgLocation,
                    r.MasterPackingSize,
                    r.OuterPackingSize,
                    r.Supervisor,
                    r.Email,
                    r.LineNo,
                    r.FanType,
                    r.ModelCode,
                    r.Size,
                    r.ColorCode,   
                    r.Labels
                );
            }

            return table;
        }
    }
}

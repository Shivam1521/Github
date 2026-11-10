using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScratchController(ILoggerManager _logger, IWrapperRepository repo, AppSecrets secrets, EnvironmentVariables variables, IWebHostEnvironment env) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        /// <summary>
        /// To Insert Scratch Details against SO Number
        /// </summary>
        /// <param name="request"><
        /// ram>
        /// <returns></returns>
        [HttpPost("InsertScratchDetails")]
        public async Task<ActionResult<Response>> InsertScratchDetails(ScratchRequest request)
        {
            Response res = new();
            try
            {
                request.IssuedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.Scratch.InsertScratchDetails(request);
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
                _logger.LogInfo("InsertScratchDetails Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("InsertScratchDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Scratch Details to Bind in Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetScratchDetails")]
        public async Task<ActionResult<HttpResponse<ScratchDetails>>> GetScratchDetails(TableRequest request)
        {
            HttpResponse<ScratchDetails> res = new();
            try
            {
                var result = await repo.Scratch.GetScratchDetails(request);
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
                _logger.LogInfo("GetScratchDetails Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetScratchDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        

        // ==========================================================
        // Controller
        // ==========================================================
        //[HttpGet("GetScratchFile")]
        //public async Task<ActionResult<ScratchResponse>> GetScratchFile(string SONo, CancellationToken cancellationToken)
        //{
        //    var res = new ScratchResponse();
        //    const long MAX_ALLOWED = 10_000_000; // 1 Crore hard cap
        //    const long chunkSize = 1_000_000;

        //    // Per-SONo lock — no two requests for the same SONo run concurrently
        //    var soLock = ScratchGenerationLock.GetLock(SONo);
        //    await soLock.WaitAsync(cancellationToken);

        //    // Temp working folder — kept separate from the final zip so partial
        //    // csv chunks are never visible/downloadable to a user directly.
        //    string tempWorkFolder = null;

        //    try
        //    {
        //        var result = await repo.Scratch.GetSODetails(SONo, cancellationToken);

        //        if (result.Id == 0)
        //        {
        //            res.ResponseCode = "400";
        //            res.ResponseMessage = "Invalid SO Number";
        //            return BadRequest(res);
        //        }

        //        if (result.ToNo < result.FromNo)
        //        {
        //            res.ResponseCode = "400";
        //            res.ResponseMessage = "Invalid range: ToNo is less than FromNo";
        //            return BadRequest(res);
        //        }

        //        long totalRequested = result.ToNo - result.FromNo + 1;
        //        if (totalRequested > MAX_ALLOWED)
        //        {
        //            res.ResponseCode = "400";
        //            res.ResponseMessage = $"Requested range ({totalRequested:N0}) exceeds maximum allowed limit of 1 Crore (10,000,000) records.";
        //            return BadRequest(res);
        //        }

        //        string outputRoot = Path.Combine(env.WebRootPath, "ScratchFiles");
        //        Directory.CreateDirectory(outputRoot);

        //        string zipFileName = $"{SONo}_{result.FromNo}_{result.ToNo}.zip";
        //        string zipFilePath = Path.Combine(outputRoot, zipFileName);

        //        // ---- VERIFICATION #1: zip already exists → return immediately ----
        //        if (System.IO.File.Exists(zipFilePath))
        //        {
        //            res.ResponseCode = "200";
        //            res.ResponseMessage = "File already generated previously. Returning existing file.";
        //            res.FileUrl = BuildUrl(zipFileName);
        //            return Ok(res);
        //        }

        //        // Build chunk plan
        //        var chunkList = new List<(long Start, long End, string FileName)>();
        //        for (long startNo = result.FromNo; startNo <= result.ToNo; startNo += chunkSize)
        //        {
        //            long endNo = Math.Min(startNo + chunkSize - 1, result.ToNo);
        //            chunkList.Add((startNo, endNo, $"{SONo}_{startNo}_{endNo}.csv"));
        //        }

        //        // Isolated temp folder per request-lock, so concurrent generations
        //        // (for different SONos) never collide, and this SONo's own retry
        //        // never sees another run's partial files.
        //        tempWorkFolder = Path.Combine(Path.GetTempPath(), "ScratchTemp", SONo);
        //        if (Directory.Exists(tempWorkFolder))
        //            Directory.Delete(tempWorkFolder, recursive: true); // clear any stale partial run
        //        Directory.CreateDirectory(tempWorkFolder);

        //        var parallelOptions = new ParallelOptions
        //        {
        //            MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 5),
        //            CancellationToken = cancellationToken
        //        };

        //        // ---- Step 1: Generate all chunk CSVs (streamed, low memory) ----
        //        await Parallel.ForEachAsync(chunkList, parallelOptions, async (chunk, ct) =>
        //        {
        //            string chunkPath = Path.Combine(tempWorkFolder, chunk.FileName);
        //            string tempChunkPath = chunkPath + ".tmp";

        //            await using (var stream = new FileStream(tempChunkPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, useAsync: true))
        //            await using (var writer = new StreamWriter(stream))
        //            {
        //                await writer.WriteLineAsync("SeqNo,ScratchCode".AsMemory(), ct);
        //                for (long seq = chunk.Start; seq <= chunk.End; seq++)
        //                {
        //                    ct.ThrowIfCancellationRequested();
        //                    await writer.WriteLineAsync($"{seq},{qrGenerator.GenerateScratch(seq)}".AsMemory(), ct);
        //                }
        //            }

        //            System.IO.File.Move(tempChunkPath, chunkPath, overwrite: true);
        //        });

        //        // ---- Step 2: Zip all chunk CSVs into ONE file (streamed, not loaded into memory) ----
        //        string tempZipPath = zipFilePath + ".tmp";

        //        // Delete any stale partial zip from a previous crashed attempt
        //        if (System.IO.File.Exists(tempZipPath))
        //            System.IO.File.Delete(tempZipPath);

        //        using (var zipStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
        //        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        //        {
        //            foreach (var chunk in chunkList)
        //            {
        //                cancellationToken.ThrowIfCancellationRequested();

        //                string chunkPath = Path.Combine(tempWorkFolder, chunk.FileName);
        //                var entry = archive.CreateEntry(chunk.FileName, CompressionLevel.Fastest);

        //                await using var entryStream = entry.Open();
        //                await using var chunkFileStream = new FileStream(chunkPath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, useAsync: true);
        //                await chunkFileStream.CopyToAsync(entryStream, 65536, cancellationToken);
        //            }
        //        }

        //        // ---- VERIFICATION #2: atomic publish of final zip ----
        //        // Only visible under its real name once fully written and closed.
        //        System.IO.File.Move(tempZipPath, zipFilePath, overwrite: true);

        //        // Cleanup: individual CSV chunks are no longer needed once zipped
        //        Directory.Delete(tempWorkFolder, recursive: true);
        //        tempWorkFolder = null;

        //        res.ResponseCode = "200";
        //        res.ResponseMessage = "File generated successfully";
        //        res.FileUrl = BuildUrl(zipFileName);
        //        return Ok(res);
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        res.ResponseCode = "499";
        //        res.ResponseMessage = "Request cancelled by client";
        //        return StatusCode(499, res);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInfo("GetScratchFile Request: " + SONo);
        //        _logger.LogError("GetScratchFile: " + JsonConvert.SerializeObject(ex));
        //        res.ResponseCode = "500";
        //        res.ResponseMessage = "Internal Server Error";
        //        return StatusCode(500, res);
        //    }
        //    finally
        //    {
        //        // Best-effort cleanup of temp working folder if something failed mid-way
        //        if (tempWorkFolder != null && Directory.Exists(tempWorkFolder))
        //        {
        //            try { Directory.Delete(tempWorkFolder, recursive: true); } catch { /* non-fatal */ }
        //        }

        //        ScratchGenerationLock.ReleaseAndCleanup(SONo, soLock);
        //    }

        //    string BuildUrl(string fileName) =>
        //        $"{Request.Scheme}://{Request.Host}{Request.PathBase}/ScratchFiles/{fileName}";
        //}


        
[HttpGet("GetScratchFile")]
public async Task<ActionResult<ScratchResponse>> GetScratchFile(string SONo, CancellationToken cancellationToken)
{
    var res = new ScratchResponse();
    const long MAX_ALLOWED = 10_000_000; // 1 Crore hard cap

    // ---- Panasonic multi-stream CSV layout constants ----
    const int StreamsPerGroup = 5;                                  // parallel (SeqNo,Code) column-pairs per block
    const int RowsPerStream   = 1500;                                // numbers per stream before a new block starts
    const int GroupSize       = StreamsPerGroup * RowsPerStream;     // 7500 numbers per block

    // Per-SONo lock — no two requests for the same SONo run concurrently
    var soLock = ScratchGenerationLock.GetLock(SONo);
    await soLock.WaitAsync(cancellationToken);

    // Temp working folder — kept separate from the final zip so partial
    // group csv files are never visible/downloadable to a user directly.
    string tempWorkFolder = null;

    try
    {
        var result = await repo.Scratch.GetSODetails(SONo, cancellationToken);

        if (result.Id == 0)
        {
            res.ResponseCode = "400";
            res.ResponseMessage = "Invalid SO Number";
            return BadRequest(res);
        }

        if (result.ToNo < result.FromNo)
        {
            res.ResponseCode = "400";
            res.ResponseMessage = "Invalid range: ToNo is less than FromNo";
            return BadRequest(res);
        }

        long totalRequested = result.ToNo - result.FromNo + 1;
        if (totalRequested > MAX_ALLOWED)
        {
            res.ResponseCode = "400";
            res.ResponseMessage = $"Requested range ({totalRequested:N0}) exceeds maximum allowed limit of 1 Crore (10,000,000) records.";
            return BadRequest(res);
        }

        string outputRoot = Path.Combine(env.WebRootPath, "ScratchFiles");
        Directory.CreateDirectory(outputRoot);

        // ---- File naming UNCHANGED from your current process ----
        string zipFileName  = $"{SONo}_{result.FromNo}_{result.ToNo}.zip";
        string zipFilePath  = Path.Combine(outputRoot, zipFileName);
        string csvEntryName = $"{SONo}_{result.FromNo}_{result.ToNo}.csv";

        // ---- VERIFICATION #1: zip already exists -> return immediately ----
        if (System.IO.File.Exists(zipFilePath))
        {
            res.ResponseCode = "200";
            res.ResponseMessage = "File already generated previously. Returning existing file.";
            res.FileUrl = BuildUrl(zipFileName);
            return Ok(res);
        }

        // ---- Build the group plan (cheap: at most ~1334 groups even at the 1-crore cap) ----
        var groups = new List<GroupPlan>();
        {
            long runningCounter = 0; // counter value of the last row written so far
            int groupIndex = 0;
            for (long groupStart = result.FromNo; groupStart <= result.ToNo; groupStart += GroupSize)
            {
                long groupCount = Math.Min(GroupSize, result.ToNo - groupStart + 1);
                int dataRows = (int)Math.Min(RowsPerStream, groupCount);

                var lens = new int[StreamsPerGroup];
                var starts = new long[StreamsPerGroup];
                long remaining = groupCount;
                long cursor = groupStart;
                for (int i = 0; i < StreamsPerGroup; i++)
                {
                    lens[i] = (int)Math.Min(RowsPerStream, remaining);
                    starts[i] = cursor;
                    cursor += lens[i];
                    remaining -= lens[i];
                }

                long headerCounter = runningCounter + 1;
                runningCounter = headerCounter + dataRows;

                groups.Add(new GroupPlan
                {
                    GroupIndex    = groupIndex,
                    HeaderCounter = headerCounter,
                    DataRows      = dataRows,
                    StreamStarts  = starts,
                    StreamLens    = lens,
                    FileName      = $"{SONo}_grp_{groupIndex}.csv"
                });

                groupIndex++;
            }
        }

        // Isolated temp folder per request-lock, so concurrent generations
        // (for different SONos) never collide, and this SONo's own retry
        // never sees another run's partial files.
        tempWorkFolder = Path.Combine(Path.GetTempPath(), "ScratchTemp", SONo);
        if (Directory.Exists(tempWorkFolder))
            Directory.Delete(tempWorkFolder, recursive: true); // clear any stale partial run
        Directory.CreateDirectory(tempWorkFolder);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 5),
            CancellationToken = cancellationToken
        };

        // ---- Step 1: generate each group's block (header row + up to 1500 data rows) ----
        await Parallel.ForEachAsync(groups, parallelOptions, async (group, ct) =>
        {
            string groupPath = Path.Combine(tempWorkFolder, group.FileName);
            string tempGroupPath = groupPath + ".tmp";

            await using (var stream = new FileStream(tempGroupPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, useAsync: true))
            await using (var writer = new StreamWriter(stream))
            {
                writer.NewLine = "\r\n";

                // ---- header row for this block ----
                var header = new string[30];
                for (int i = 0; i < StreamsPerGroup; i++)
                {
                    string spl = $"SPL {(group.GroupIndex * StreamsPerGroup + i + 1):000}";
                    header[2 * i] = spl;
                    header[2 * i + 1] = "";
                    header[10 + 2 * i] = spl; // ASSUMPTION: SPL label shown even if this stream is empty in the final group
                    if (2 * i + 1 < 9)
                        header[10 + 2 * i + 1] = "";
                }
                header[19] = group.HeaderCounter.ToString();
                for (int i = 0; i < StreamsPerGroup; i++)
                {
                    if (group.StreamLens[i] > 0)
                    {
                        header[20 + 2 * i] = group.StreamStarts[i].ToString();
                        header[21 + 2 * i] = (group.StreamStarts[i] + group.StreamLens[i] - 1).ToString();
                    }
                    else
                    {
                        header[20 + 2 * i] = "";
                        header[21 + 2 * i] = "";
                    }
                }
                await writer.WriteLineAsync(string.Join(",", header).AsMemory(), ct);

                // ---- data rows ----
                var row = new string[30];
                for (int r = 1; r <= group.DataRows; r++)
                {
                    ct.ThrowIfCancellationRequested();

                    for (int i = 0; i < StreamsPerGroup; i++)
                    {
                        if (r <= group.StreamLens[i])
                        {
                            long seq = group.StreamStarts[i] + (r - 1);
                            row[2 * i] = seq.ToString();
                            row[2 * i + 1] = qrGenerator.GenerateScratch(seq);
                        }
                        else
                        {
                            row[2 * i] = "";
                            row[2 * i + 1] = "";
                        }
                    }
                    for (int c = 10; c <= 18; c++) row[c] = "";
                    row[19] = (group.HeaderCounter + r).ToString();
                    for (int c = 20; c <= 29; c++) row[c] = "";

                    await writer.WriteLineAsync(string.Join(",", row).AsMemory(), ct);
                }
            }

            System.IO.File.Move(tempGroupPath, groupPath, overwrite: true);
        });

        // ---- Step 2: concatenate all group files into ONE csv, inside the zip (streamed, not loaded into memory) ----
        string tempZipPath = zipFilePath + ".tmp";

        // Delete any stale partial zip from a previous crashed attempt
        if (System.IO.File.Exists(tempZipPath))
            System.IO.File.Delete(tempZipPath);

        using (var zipStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            var entry = archive.CreateEntry(csvEntryName, CompressionLevel.Fastest);
            await using var entryStream = entry.Open();
            await using (var entryWriter = new StreamWriter(entryStream))
            {
                entryWriter.NewLine = "\r\n";

                // Fixed top-level column header — required by the Panasonic layout (note the
                // trailing space after "START NO. 1" and the run of blank columns 10-19;
                // this exact text matches your sample file byte-for-byte).
                await entryWriter.WriteLineAsync(
                    "con 1,sc 1,con 2,sc 2,con 3,sc 3,con 4,sc 4,con 5,sc 5,,,,,,,,,,,START NO. 1 ,END NO. 1,START NO. 2,END NO. 2,START NO. 3,END NO. 3,START NO. 4,END NO. 4,START NO. 5,END NO. 5"
                        .AsMemory(),
                    cancellationToken);
                await entryWriter.FlushAsync();

                foreach (var group in groups)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string groupPath = Path.Combine(tempWorkFolder, group.FileName);
                    using var groupFileStream = new FileStream(groupPath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, useAsync: true);
                    using var groupReader = new StreamReader(groupFileStream);

                    string line;
                    while ((line = await groupReader.ReadLineAsync()) != null)
                    {
                        await entryWriter.WriteLineAsync(line.AsMemory(), cancellationToken);
                    }
                }
            }
        }

        // ---- VERIFICATION #2: atomic publish of final zip ----
        // Only visible under its real name once fully written and closed.
        System.IO.File.Move(tempZipPath, zipFilePath, overwrite: true);

        // Cleanup: individual group CSV chunks are no longer needed once zipped
        Directory.Delete(tempWorkFolder, recursive: true);
        tempWorkFolder = null;

        res.ResponseCode = "200";
        res.ResponseMessage = "File generated successfully";
        res.FileUrl = BuildUrl(zipFileName);
        return Ok(res);
    }
    catch (OperationCanceledException)
    {
        res.ResponseCode = "499";
        res.ResponseMessage = "Request cancelled by client";
        return StatusCode(499, res);
    }
    catch (Exception ex)
    {
        _logger.LogInfo("GetScratchFile Request: " + SONo);
        _logger.LogError("GetScratchFile: " + JsonConvert.SerializeObject(ex));
        res.ResponseCode = "500";
        res.ResponseMessage = "Internal Server Error";
        return StatusCode(500, res);
    }
    finally
    {
        // Best-effort cleanup of temp working folder if something failed mid-way
        if (tempWorkFolder != null && Directory.Exists(tempWorkFolder))
        {
            try { Directory.Delete(tempWorkFolder, recursive: true); } catch { /* non-fatal */ }
        }

        ScratchGenerationLock.ReleaseAndCleanup(SONo, soLock);
    }

    string BuildUrl(string fileName) =>
        $"{Request.Scheme}://{Request.Host}{Request.PathBase}/ScratchFiles/{fileName}";
}

// Small helper struct describing one 7500-number block of the output.
private class GroupPlan
{
    public int GroupIndex;
    public long HeaderCounter;
    public int DataRows;
    public long[] StreamStarts;
    public int[] StreamLens;
    public string FileName;
}

    }
}

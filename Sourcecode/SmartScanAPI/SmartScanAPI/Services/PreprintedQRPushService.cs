using Entities;
using IRepository;
using Logger;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using SmartScanAPI.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartScanAPI.Services
{
    public class PreprintedQRPushService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        AppSecrets secrets,
        EnvironmentVariables variables) : BackgroundService
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            Console.WriteLine("PREPRINTED QR SERVICE STARTED ");
            using var scope = scopeFactory.CreateScope();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerManager>();

            var enabled = configuration["APEXQRConfig:Enabled"];

            var intervalMinutes = configuration.GetValue<int>(
                "APEXQRConfig:IntervalInMinutes",
                1);

            if (!string.Equals(
                enabled,
                "ON",
                StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInfo(
                    "Preprinted QR Push Service is disabled. " +
                    "APEXQRConfig:Enabled = " + enabled);

                return;
            }

            logger.LogInfo(
                "Preprinted QR Push Service Started. " +
                "IntervalInMinutes = " + intervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRecords(
                        stoppingToken,
                        logger);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        "Preprinted QR Push Service Error: " +
                        JsonConvert.SerializeObject(ex));
                }

                try
                {
                    await Task.Delay(
                        TimeSpan.FromMinutes(intervalMinutes),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            logger.LogInfo(
                "Preprinted QR Push Service Stopped.");
        }

        private async Task ProcessRecords(
            CancellationToken cancellationToken,
            ILoggerManager logger)
        {
            using var scope = scopeFactory.CreateScope();

            var repo = scope.ServiceProvider
                .GetRequiredService<IPreprintedQRJobRepository>();

            var tokenGenerator = scope.ServiceProvider
                .GetRequiredService<EBSTokenGenerator>();

            //logger.LogInfo(
            //    "Checking pending Preprinted QR records.");

            // Get maximum 1000 pending records
            var records = (await repo.GetPendingPreprintedQRForPush())
                .ToList();

            if (records.Count == 0)
            {
                //logger.LogInfo(
                //    "No pending Preprinted QR records found.");

                return;
            }

            logger.LogInfo(
                "Found " + records.Count +
                " pending Preprinted QR records.");

            var ids = string.Join(
                ",",
                records.Select(x => x.Id));

            logger.LogInfo(
                "Processing Preprinted QR IDs: " + ids);

            // Prepare request body
            var request = new ClientPushRequest
            {
                items = records.Select(x => new ClientPushItem
                {
                    itemCode = x.itemCode,

                    mfgMonth = x.mfgMonth,

                    // Generate encrypted Master QR
                    masterqrCode =
                        qrGenerator.GeneratePrePrintedTertiaryQR(
                            x.srNo,
                            x.srPrefix),

                    srNo = x.srNo,
                    srPrefix = x.srPrefix,
                    qrType = x.qrType,
                    outerQrCode = x.outerQrCode,
                    supplierCode = x.supplierCode,
                    masterQuantity = x.masterQuantity,
                    primaryProductQrCode = x.primaryProductQrCode,
                    primaryPackQRCode = x.primaryPackQRCode,
                    outerBoxQuantity = x.outerBoxQuantity

                }).ToList()
            };

            logger.LogInfo(
                "Preprinted QR request prepared. " +
                "RecordCount = " + request.items.Count);

            // -------------------------------------------------
            // Increase Push Attempt Count
            // -------------------------------------------------

            try
            {
                await repo.UpdatePreprintedQRPushAttempt(ids);

                logger.LogInfo(
                    "Push attempt count updated successfully. " +
                    "IDs = " + ids);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "Failed to update Preprinted QR push attempt. " +
                    "IDs = " + ids +
                    " Error = " +
                    JsonConvert.SerializeObject(ex));

                throw;
            }

            // -------------------------------------------------
            // Generate EBS Token
            // -------------------------------------------------

            var token = await tokenGenerator.GetToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                logger.LogError(
                    "Unable to generate EBS token.");

                await repo.UpdatePreprintedQRPushResponse(
                    ids,
                    "Unable to generate EBS token.");

                logger.LogInfo(
                    "Preprinted QR response/status updated " +
                    "for token failure. IDs = " + ids);

                return;
            }

            logger.LogInfo(
                "EBS token generated successfully.");

            // -------------------------------------------------
            // Get APEX API URL
            // -------------------------------------------------

            var url = configuration[
                "APEXQRConfig:APIEndpoint"];

            if (string.IsNullOrWhiteSpace(url))
            {
                logger.LogError(
                    "APEXQRConfig:APIEndpoint is not configured.");

                await repo.UpdatePreprintedQRPushResponse(
                    ids,
                    "APEXQRConfig:APIEndpoint is not configured.");

                logger.LogInfo(
                    "Preprinted QR response/status updated " +
                    "for API URL failure. IDs = " + ids);

                return;
            }

            using var httpClient = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var json = JsonConvert.SerializeObject(request);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            // -------------------------------------------------
            // Call APEX API
            // -------------------------------------------------

            try
            {
                logger.LogInfo(
                    "Sending " + records.Count +
                    " Preprinted QR records to APEX API. " +
                    "URL = " + url);

                var response = await httpClient.PostAsync(
                    url,
                    content,
                    cancellationToken);

                var responseBody =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                logger.LogInfo(
                    "APEX API response received. " +
                    "StatusCode = " +
                    (int)response.StatusCode +
                    ", Response = " +
                    responseBody);

                // -------------------------------------------------
                // Success - HTTP 202
                // -------------------------------------------------

                if (response.StatusCode ==
                    HttpStatusCode.Accepted)
                {
                    logger.LogInfo(
                        "APEX API returned 202. " +
                        records.Count +
                        " records pushed successfully.");

                    await repo.UpdatePreprintedQRPushResponse(
                        ids,
                        "HTTP 202: " + responseBody);

                    logger.LogInfo(
                        "Preprinted QR push response/status " +
                        "updated successfully. IDs = " + ids);
                }

                // -------------------------------------------------
                // Unexpected Status Code
                // -------------------------------------------------

                else
                {
                    logger.LogError(
                        "APEX API returned unexpected status code. " +
                        "StatusCode = " +
                        (int)response.StatusCode +
                        ", Response = " +
                        responseBody);

                    await repo.UpdatePreprintedQRPushResponse(
                        ids,
                        "HTTP " +
                        (int)response.StatusCode +
                        ": " +
                        responseBody);

                    logger.LogInfo(
                        "Preprinted QR failure response/status " +
                        "updated. IDs = " + ids);
                }
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                logger.LogError(
                    "Preprinted QR Push Service cancellation requested.");

                throw;
            }
            catch (Exception ex)
            {
                var error =
                    JsonConvert.SerializeObject(ex);

                logger.LogError(
                    "APEX QR API call failed. " +
                    "IDs = " + ids +
                    ", Error = " + error);

                // -------------------------------------------------
                // Update DB with API Exception
                // -------------------------------------------------

                try
                {
                    await repo.UpdatePreprintedQRPushResponse(
                        ids,
                        "API Exception: " + error);

                    logger.LogInfo(
                        "Preprinted QR API failure response/status " +
                        "updated. IDs = " + ids);
                }
                catch (Exception dbEx)
                {
                    logger.LogError(
                        "Failed to update Preprinted QR failure " +
                        "response. IDs = " + ids +
                        ", Error = " +
                        JsonConvert.SerializeObject(dbEx));
                }
            }
        }
    }
}
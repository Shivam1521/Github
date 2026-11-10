using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.Security.Cryptography;

namespace SmartScanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRAuthenticationController(IWrapperRepository repo, ILoggerManager _logger, AppSecrets secrets, EnvironmentVariables variables) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        /// <summary>
        /// To Get QR Details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("VerifyQR")]
        public async Task<ActionResult<SingleResponse<QRAuthenticationResponse>>> VerifyQR(QRAuthenticationRequest request)
        {
            SingleResponse<QRAuthenticationResponse> res = new();
            bool isException = false;
            try
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.QRData))
                        throw new ArgumentException("QRData cannot be null or empty.", nameof(request.QRData));

                    if (request.QRData.Contains(":%5D"))
                    {
                        request.QRData = request.QRData.Replace(":%5D", ":]");
                    }

                    // Ensure the QRData contains the expected delimiter
                    var splitData = request.QRData.Split("pa?uid=:]C1");
                    if (splitData.Length == 0 || string.IsNullOrEmpty(splitData[1]))
                    {
                        request.SeqNo = -1;
                        _logger.LogError("QRData format is invalid. Missing expected delimiter 'pa?uid=:]C1' in VerifyQR: " + request.QRData);
                    }

                    var cleanedData = splitData[1].Replace(":", "");

                    var decryptedString = qrGenerator.Decrypt(cleanedData);

                    if (string.IsNullOrWhiteSpace(decryptedString) || decryptedString.Length < 3)
                    {
                        isException = true;
                        request.SeqNo = -1;
                        _logger.LogError("Decrypted string is invalid or too short in VerifyQR: " + request.QRData);
                    }

                    var prefixChar = decryptedString[0];  // safer than slicing
                    request.Prefix = decryptedString.Substring(1, 2);

                    if (!long.TryParse(decryptedString[3..], out var seqNo))
                    {
                        isException = true;
                        request.SeqNo = -1;
                        _logger.LogError("Decrypted string does not contain a valid sequence number in VerifyQR: " + request.QRData);
                    }
                    else
                    {
                        request.SeqNo = seqNo;
                    }

                    // Map type safely
                    request.QRType = prefixChar switch
                    {
                        var c when c.ToString() == PassPhrases.SecondaryPacking => "O",
                        var c when c.ToString() == PassPhrases.TertiaryPacking => "M",
                        var c when c.ToString() == PassPhrases.PrePrintedSecondary => "D",
                        var c when c.ToString() == PassPhrases.PrePrintedTertiary => "L",
                        _ => "U" // Unknown
                    };
                }
                catch (ArgumentException argEx)
                {
                    isException = true;
                    request.SeqNo = -1;
                    _logger.LogError("Invalid argument in VerifyQR: " + request.QRData + JsonConvert.SerializeObject(argEx));
                }
                catch (FormatException fmtEx)
                {
                    isException = true;
                    request.SeqNo = -1;
                    _logger.LogError("Invalid QRData format in VerifyQR: " + request.QRData + JsonConvert.SerializeObject(fmtEx));
                }
                catch (CryptographicException cryptoEx)
                {
                    isException = true;
                    request.SeqNo = -1;
                    _logger.LogError("Decryption failed in VerifyQR: " + request.QRData + JsonConvert.SerializeObject(cryptoEx));
                }
                catch (Exception ex)
                {
                    isException = true;
                    request.SeqNo = -1;
                    _logger.LogError("Unexpected error in VerifyQR: " + request.QRData + JsonConvert.SerializeObject(ex));
                }

                if (isException)
                {
                    var result = await repo.QRAuthentication.InsertVerificationLog(request);
                    return BadRequest(result);
                }
                else
                {
                    var result = await repo.QRAuthentication.VerifyQR(request);
                    if (result.ResponseCode == "200")
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("VerifyQR Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("VerifyQR: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}

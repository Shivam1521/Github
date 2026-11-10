using Entities;
using Logger;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace SmartScanAPI.Helper
{
    public static class CommonMethods
    {
        public static void ToCsv<T>(IEnumerable<T> items, string filePath, params string[] skipColumns)
        {
            var sb = new StringBuilder();

            // Normalize skip list (case-insensitive)
            var skipSet = new HashSet<string>(
                skipColumns ?? [],
                StringComparer.OrdinalIgnoreCase
            );

            // Filter properties
            var props = typeof(T)
                .GetProperties()
                .Where(p => !skipSet.Contains(p.Name))
                .ToArray();

            // Header
            sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

            // Rows
            foreach (var item in items)
            {
                var values = new List<string>();

                foreach (var prop in props)
                {
                    var value = prop.GetValue(item, null)?.ToString() ?? "";

                    // Escape commas & quotes
                    if (value.Contains(',') || value.Contains('"'))
                        value = $"\"{value.Replace("\"", "\"\"")}\"";

                    values.Add(value);
                }

                sb.AppendLine(string.Join(",", values));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public static QRParseResult ParseQR(string qrData, ILoggerManager _logger, QRGenerator qrGenerator, string APIName)
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
                        _logger.LogError($"{APIName}: Invalid QR format: {qrData}");
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
                    _logger.LogError($"{APIName}: Invalid decrypted data for QR: {qrData}");
                    return result;
                }

                // Extract prefix and sequence
                char qrPrefixChar = decrypted[0];
                result.Prefix = decrypted.Substring(1, 2);

                if (!long.TryParse(decrypted[3..], out long seqNo))
                {
                    result.ParseStatus = "Failed to parse sequence number.";
                    _logger.LogError($"{APIName}: Failed to parse sequence number in QR: {qrData}");
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
                    var c when c.ToString() == PassPhrases.PrePrintedBlade => PassPhrases.PrePrintedBlade,
                    _ => "U" // Unknown type
                };

                result.ParseStatus = "Success";
            }
            catch (CryptographicException ex)
            {
                result.ParseStatus = "Decryption failed.";
                _logger.LogError($"{APIName}: Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                result.ParseStatus = "Unexpected error during parsing.";
                _logger.LogError($"{APIName}: Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }

            return result;
        }

        public static QRParseResult ParseSeqQR(string qrData, ILoggerManager _logger, QRGenerator qrGenerator, string APIName)
        {
            var result = new QRParseResult();

            try
            {
                if (string.IsNullOrWhiteSpace(qrData))
                {
                    result.ParseStatus = "QR data is null or empty.";
                    return result;
                }

            //    var decrypted = qrGenerator.Decrypt(qrData);

                if (string.IsNullOrWhiteSpace(qrData) || qrData.Length < 4)
                {
                    result.ParseStatus = "Invalid decrypted data.";
                    _logger.LogError($"{APIName}: Invalid decrypted data for QR: {qrData}");
                    return result;
                }

                // Extract prefix and sequence
                char qrPrefixChar = qrData[0];
                result.Prefix = qrData.Substring(1, 2);

                if (!long.TryParse(qrData[3..], out long seqNo))
                {
                    result.ParseStatus = "Failed to parse sequence number.";
                    _logger.LogError($"{APIName}: Failed to parse sequence number in QR: {qrData}");
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
                _logger.LogError($"{APIName}: Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                result.ParseStatus = "Unexpected error during parsing.";
                _logger.LogError($"{APIName}: Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }

            return result;
        }

        public static void DumpToCsv(List<PrintedSeq> data, string filePath)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("QR,QRType,ItemCode,Quantity");

            // Rows
            foreach (var item in data)
            {
                sb.AppendLine($"{Escape(item.QR)},{Escape(item.QRType)},{Escape(item.ItemCode)},{Escape(item.Qty)}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string Escape(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                value = $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}

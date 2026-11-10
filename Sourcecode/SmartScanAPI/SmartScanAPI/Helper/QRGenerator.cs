using System.Text;
using Entities;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Fpe;
using Org.BouncyCastle.Crypto.Parameters;

namespace SmartScanAPI.Helper
{
    /// <summary>
    /// 14-char, uppercase+digits, excluding 0/O/I, FF1 (AES) format-preserving encryption.
    /// Deterministic & reversible: same input -> same output, and Decrypt(Encrypt(x)) == x.
    /// </summary>
    public class QRGenerator
    {
        private readonly byte[] _key;
        private readonly byte[] _tweak;
        private readonly string URL;

        public QRGenerator(AppSecrets secrets, EnvironmentVariables variables)
        {
            if (string.IsNullOrWhiteSpace(secrets.Hex))
                throw new ArgumentException("Hex secret must not be empty.", "Hex");
            if (string.IsNullOrWhiteSpace(secrets.Tweak))
                throw new ArgumentException("Tweak must not be empty.", "Tweak");

            // Make sure the key is exactly 32 bytes (AES-256)
            _key = Encoding.UTF8.GetBytes(secrets.Hex.PadRight(32))[..32];

            // Make sure the tweak is at most 16 bytes
            _tweak = Encoding.UTF8.GetBytes(secrets.Tweak.PadRight(16))[..16];

            if (string.IsNullOrWhiteSpace(variables.Name))
                throw new ArgumentException("Environment Name must not be empty.", "Name");

            if (variables.Name.Equals("dev", StringComparison.CurrentCultureIgnoreCase)
                || variables.Name.Equals("development", StringComparison.CurrentCultureIgnoreCase))
            {
                URL = "https://smartscan.holostikapp.com/";
            }
            else
            {
                URL = variables.Details.QRURL;
            }
        }

        public string GeneratePrimaryQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.Primary + prefix + srNo.ToString().PadLeft(11, 'P'));
        }

        public string GeneratePrimaryPackingQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.PrimaryPacking + prefix + srNo.ToString().PadLeft(11, 'P'));
        }

        public string GenerateSecondaryQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.SecondaryPacking + prefix + srNo.ToString().PadLeft(11, 'P'));
        }

        public string GenerateTertiaryQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.TertiaryPacking + prefix + srNo.ToString().PadLeft(11, 'P'));
        }

        public string GeneratePrePrintedSecondaryQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.PrePrintedSecondary + prefix + srNo.ToString().PadLeft(11, 'P'));
        }

        public string GeneratePrePrintedTertiaryQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.PrePrintedTertiary + prefix + srNo.ToString().PadLeft(11, 'P'));
        }
        public string GeneratePrePrintedBladeQR(long srNo, string prefix)
        {
            return Encrypt(PassPhrases.PrePrintedBlade + prefix + srNo.ToString().PadLeft(11, 'P'));
        }

        public string GenerateScratch(long srNo)
        {
            return Encrypt(srNo.ToString().PadLeft(10, 'P'), true);
        }

        public const string Alphabet = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

        public string Encrypt(string plain14, bool skip14 = false)
        {
            if (!skip14)
                Validate(plain14);

            var engine = new FpeFf1Engine(new AesEngine());
            engine.Init(true, new FpeParameters(new KeyParameter(_key), Alphabet.Length, _tweak));

            // convert chars -> byte indexes
            byte[] input = ToAlphabetBytes(plain14);
            byte[] output = new byte[input.Length];

            engine.ProcessBlock(input, 0, input.Length, output, 0);

            return FromAlphabetBytes(output);
        }

        public string Decrypt(string cipher14)
        {
            Validate(cipher14);

            var engine = new FpeFf1Engine(new AesEngine());
            engine.Init(false, new FpeParameters(new KeyParameter(_key), Alphabet.Length, _tweak));

            byte[] input = ToAlphabetBytes(cipher14);
            byte[] output = new byte[input.Length];

            engine.ProcessBlock(input, 0, input.Length, output, 0);

            var decryptedString = FromAlphabetBytes(output);

            return ReplaceAllInLast11(decryptedString, 'P', '0');
        }

        public string ReplaceAllInLast11(string input, char oldChar, char newChar)
        {
            if (input.Length != 14)
                throw new ArgumentException("Input must be 14 characters long.");

            string firstPart = input[..3];  // first 3 chars
            string lastPart = input.Substring(3, 11);   // last 11 chars

            // Replace only in last part
            lastPart = lastPart.Replace(oldChar, newChar);

            return firstPart + lastPart;
        }

        private static void Validate(string s)
        {
            if (s == null || s.Length != 14)
                throw new ArgumentException("Input must be exactly 14 characters.");

            foreach (char c in s)
                if (!Alphabet.Contains(c))
                    throw new ArgumentException($"Invalid character '{c}'. Allowed: {Alphabet}");
        }

        private static byte[] ToAlphabetBytes(string text)
        {
            byte[] result = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                int idx = Alphabet.IndexOf(text[i]);
                if (idx < 0) throw new ArgumentException($"Invalid char '{text[i]}'");
                result[i] = (byte)idx;
            }
            return result;
        }

        private static string FromAlphabetBytes(byte[] indexes)
        {
            char[] result = new char[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
                result[i] = Alphabet[indexes[i]];
            return new string(result);
        }
    }
}

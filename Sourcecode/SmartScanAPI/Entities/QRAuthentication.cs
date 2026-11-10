namespace Entities
{
    public class QRAuthenticationRequest
    {
        public required string QRData { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public long SeqNo { get; set; }
        public required string VerificationMode { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public required string QRType { get; set; }
    }

    public class QRAuthenticationResponse
    {

    }

    public class QRResult
    {
        public required long SeqNo { get; set; }
        public required string Prefix { get; set; }
        public required string QRType { get; set; }
        public required string Reason { get; set; } = string.Empty;
    }
}

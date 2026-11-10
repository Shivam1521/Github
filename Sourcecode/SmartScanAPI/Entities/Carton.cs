using System.Text.Json.Serialization;

namespace Entities
{
    public class ReceiveCartonsRequest
    {
        public required string ReceivingLocation { get; set; }
        public required List<QRCode> QRs { get; set; }
    }

    public class SingleReceiveCartonsRequest
    {
        public required string ReceivingLocation { get; set; }
        public required string QR { get; set; }
    }

    public class SubmitReceiveCartonsRequest
    {
        public required string PickSlipNumber { get; set; }
    }

    public class DispatchCartonsRequest
    {
        public required string PickSlipNumber { get; set; }
        public required List<DispatchQRCodeDetail> QRs { get; set; }
    }

    public class VerifyDispatchCartonsRequest
    {
        public required string PickSlipNumber { get; set; }
        public required List<DispatchQRCodeDetail> QRs { get; set; }
    }

    public class SubmitDispatchRequest
    {
        public string PickSlipNumber { get; set; }
    }

    public class QRCode
    {
        public required string QR { get; set; }
    }

    public class DispatchQRCodeDetail
    {
        public required string QR { get; set; }
        public required string PickType { get; set; }
        public required int Qty { get; set; }
        public List<OuterDetails> Outers { get; set; } = [];
    }

    public class OuterDetails
    {
        public required string ItemCode { get; set; }
        public required string QR { get; set; }
        public required int Qty { get; set; }
        [JsonIgnore]
        public long SeqNo { get; set; }
        [JsonIgnore]
        public string Prefix { get; set; } = string.Empty;
        [JsonIgnore]
        public string QRType { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
    }

    public class ReceiveCarton
    {
        public required long SeqNo { get; set; }
        public required string Prefix { get; set; }
        public required string QRType { get; set; }
        public required string ReceivingLocation { get; set; }
        public required int PerformedBy { get; set; }
    }

    public class DispatchCarton
    {
        public required long SeqNo { get; set; }
        public required string Prefix { get; set; }
        public required string QRType { get; set; }
        public required string PickSlipNumber { get; set; }
        public required string PickType { get; set; }
        public required int PerformedBy { get; set; }
        public List<OuterDetails> Outers { get; set; } = [];
    }

    public class ReceiveCartonResult
    {
        [JsonIgnore]
        public int RowId { get; set; }
        public required string QR { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
    }

    public class DispatchCartonTVP
    {
        public required int RowId { get; set; }
        public required long SeqNo { get; set; }
        public required string Prefix { get; set; }
        public required string QRType { get; set; }
        public required string QR { get; set; }
    }

    public class DispatchCartonResult
    {
        [JsonIgnore]
        public int RowId { get; set; }
        public required string QR { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
    }

    public class QRParseResult
    {
        public long SeqNo { get; set; } = -1;
        public string Prefix { get; set; } = string.Empty;
        public string QRType { get; set; } = "U";
        public string ParseStatus { get; set; } = string.Empty; // error message or reason
    }

    public class CartonDetailsForAttachDetachRequest
    {
        public required string QR { get; set; }
        public required string ActionFor { get; set; }
        public required string ActionType { get; set; }
        public long SeqNo { get; set; }
        public string QRType { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
    }

    public class CartonDetailsForAttachDetach
    {
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Qty { get; set; }
        public long SeqNo { get; set; }
        public string QRType { get; set; } = string.Empty;
        public int UnitId { get; set; }
    }

    public class OuterQRsResult
    {
        public int TotalQty { get; set; }
        public int NoOfOuters { get; set; }
        public List<OuterQRs> Outers { get; set; } = [];
    }

    public class OuterDataQRsResult
    {
        public int TotalQty { get; set; }
        public int NoOfOuters { get; set; }
        public List<OuterDataQRs> Outers { get; set; } = [];
    }

    public class OuterQRs
    {
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
        [JsonIgnore]
        public long SeqNo { get; set; }
        [JsonIgnore]
        public string Prefix { get; set; } = string.Empty;
        [JsonIgnore]
        public string QRType { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string OuterQR { get; set; } = string.Empty;
        public string OuterQRURL { get; set; } = string.Empty;
        public int Qty { get; set; }
    }

    public class OuterDataQRs
    {
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
        public long SeqNo { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string QRType { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string OuterQR { get; set; } = string.Empty;
        public string OuterQRURL { get; set; } = string.Empty;
        public int Qty { get; set; }
    }

    public class GetQRDetailsRequest
    {
        public required List<string> QRs { get; set; }
    }

    public class QRList
    {
        public required string QR { get; set; }
    }

    public class GetQRDetailsResponse
    {
        public string ItemCode { get; set; } = string.Empty;
        public required string QR { get; set; }
        public int Qty { get; set; }
        public string QRType { get; set; } = string.Empty;
        public string MFGMonth { get; set; } = string.Empty;
        public required string Status { get; set; }
        public required bool ParseStatus { get; set; }
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
    }
}

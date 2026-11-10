using System.Text.Json.Serialization;

namespace Entities
{
    public class CartonDetails
    {
        public string ItemCode { get; set; } = string.Empty;
        public int Qty { get; set; }
        public required long SeqNo { get; set; }
        public string QRType { get; set; } = string.Empty;
        public int UnitId { get; set; }
    }

    public class ReplaceCartonRequest
    {
        public required long FirstSeqNo { get; set; }
        public required string FirstQRType { get; set; }
        public required int FirstUnitId { get; set; }
        public required long SecondSeqNo { get; set; }
        public required string SecondQRType { get; set; }
        public required int SecondUnitId { get; set; }
        public int PerformedBy { get; set; }
    }

    public class QCStatusDetails
    {
        public string ItemCode { get; set; } = string.Empty;
        public int Qty { get; set; }
        public long SeqNo { get; set; }
        public int UnitId { get; set; }
        public string QRType { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
    }

    public class MarkQCOpenRequest
    {
        public List<QRInfoModel> QRInfo { get; set; } = [];
        public int PerformedBy { get; set; }
    }

    public class QRInfoModel
    {
        public long SeqNo { get; set; }
        public int UnitId { get; set; }
        public string QRType { get; set; } = string.Empty;
    }

    public class JobDetailsForQC
    {
        public long JobId { get; set; }
        public string JobNo { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public int Qty { get; set; }
        public string BatchCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
    }

    public class UpdateQCStatusRequest
    {
        public required long JobId { get; set; }
        public required int QCStatus { get; set; }
        public int PerformedBy { get; set; }
    }

    public class CartonDetailsForQC
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public int RequiredQty { get; set; }
        public required long SeqNo { get; set; }
        public string QRType { get; set; } = string.Empty;
        public int UnitId { get; set; }
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
    }

    public class ReplaceOuterAfterQCRequest
    {
        public required long MasterSeqNo { get; set; }
        public required string MasterQRType { get; set; }
        public required int MasterUnitId { get; set; }
        public List<QRInfoModel> QRInfo { get; set; } = [];
        public int PerformedBy { get; set; }
    }
}

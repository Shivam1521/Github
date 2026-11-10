using System.Text.Json.Serialization;

namespace Entities
{
    public class PrintJob
    {
        public int JobId { get; set; }
        public string JobNo { get; set; } = string.Empty;
        public string SrNo { get; set; } = string.Empty;
        public long FromNoOuter { get; set; }
        public long ToNoOuter { get; set; }
        public long FromNoMaster { get; set; }
        public long ToNoMaster { get; set; }
        public long LastOuterPrinted { get; set; }
        public long LastMasterPrinted { get; set; }
    }

    public class PrintJobDetails
    {
        public int JobId { get; set; }
        public string JobNo { get; set; } = string.Empty;
        public string SrNo { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int QtyOuter { get; set; }
        public int QtyMaster { get; set; }
    }

    public class PrintJobRequest
    {
        public required string SrNo { get; set; }
        public int UserId { get; set; }
        public required int Quantity { get; set; }
    }

    public class PrintJobResponse
    {
        public string JobNo { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int QtyOuter { get; set; }
        public int QtyMaster { get; set; }
        public string BatchCode { get; set; } = string.Empty;
    }

    public class LastPrintedNumberDetails
    {
        public required string ItemCode { get; set; }
        public long MasterSeqNo { get; set; }
        public required string Prefix { get; set; }
        public required string OuterQRDetails { get; set; }
        public required bool MasterExists { get; set; }
        public decimal OuterBoxMinWeight { get; set; }
        public decimal OuterBoxMaxWeight { get; set; }
    }

    public class QR
    {
        public required string ItemCode { get; set; }
        public required long MasterSeqNo { get; set; }
        public required string MasterQR { get; set; }
        public required string Prefix { get; set; }
        public required string Postfix { get; set; }
        public required string QRURL { get; set; }
        public required bool MasterExists { get; set; }
        public decimal OuterBoxMinWeight { get; set; }
        public decimal OuterBoxMaxWeight { get; set; }
        public required string MasterPrefix { get; set; }
        public required string MasterQRType { get; set; }
        public List<OuterQRDetails> OuterQRs { get; set; } = [];
    }

    public class OuterQR
    {
        public required long SeqNo { get; set; }
    }

    public class OuterQRDetails
    {
        public required long SeqNo { get; set; }
        public required string OuterQRData { get; set; }
        public required string QRType { get; set; }
        public required string QRPrefix { get; set; }

    }

    public class PrintJobStatusUpdateRequest
    {
        public required long JobId { get; set; }
        public required string Type { get; set; }
        public required long SeqNo { get; set; }
        public int UserId { get; set; }
        public string? Weight { get; set; }
        public string? PrinterIP { get; set; }
    }

    public class MasterReprintResponse
    {
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;

        public string QR { get; set; } = string.Empty;

        public string masterQR { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public long SeqNo { get; set; }
       // [JsonIgnore]
        public string QRType { get; set; } = string.Empty;
       // [JsonIgnore]
        public string Prefix { get; set; } = string.Empty;
    }

    public class ProductionLineQRsRequest
    {
        public required string ItemCode { get; set; }
        public required int Quantity { get; set; }
        public int UserId { get; set; }
    }

    public class ProductionLineQRsResponse
    {
        public required string ItemCode { get; set; }
        public long MasterSeqNo { get; set; }
        public required string Prefix { get; set; }
        public decimal OuterBoxMinWeight { get; set; }
        public decimal OuterBoxMaxWeight { get; set; }
    }

    public class PLQR
    {
        public required string ItemCode { get; set; }
        public required string MasterQR { get; set; }
    }
}

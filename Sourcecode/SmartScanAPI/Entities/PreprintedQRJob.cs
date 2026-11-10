using System.Text.Json.Serialization;

namespace Entities
{
    public class PreprintedQRDataRequest
    {
        public required int UserId { get; set; }
        public required string QRType { get; set; }
        public bool IsExistingStock { get; set; }
    }

    public class PreprintedsupplierQRDataRequest
    {
        [JsonIgnore]
        public  int UserId { get; set; }
        public required string QRType { get; set; }
    }
    public class PreprintedQRJobRequest
    {
        public required int Qty { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public required string QRType { get; set; }
        public required bool IsExistingStock { get; set; }
        public int UserId { get; set; }
    }


    public class PreprintedQRJobSupplierRequest
    {
        public required int Qty { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public required string QRType { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }

    public class PreprintedQRMappingRequest
    {
        public string OuterQr { get; set; } = string.Empty;
        public string MasterQr { get; set; } = string.Empty;
        public string BladeQr { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        [JsonIgnore]
        public int UserId { get; set; }
    }

    public class PreprintedQRMappingDBRequest
    {
        public string OuterQRType { get; set; } = string.Empty;
        // public string OuterPrefix { get; set; } = string.Empty;
        // public long OuterSrNo { get; set; }
        public string OuterSrNo { get; set; } = string.Empty;
        public string MasterQRType { get; set; } = string.Empty;
        public string MasterPrefix { get; set; } = string.Empty;
        public long MasterSrNo { get; set; }
        public string BladeQRType { get; set; } = string.Empty;
        public string BladePrefix { get; set; } = string.Empty;
        public long BladeSrNo { get; set; }

        public string Type { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
    }
    public class PreprintedQRDataResponse
    {
        public required int Id { get; set; }
        public required string QRJobId { get; set; }
        public required string Type { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDesc { get; set; } = string.Empty;
        public required bool IsExistingStock { get; set; }
        public required long FromNo { get; set; }
        public required long ToNo { get; set; }
        public required long Qty { get; set; }
        public required bool IsPrinted { get; set; }
        public required string UnitPrefix { get; set; }
        public DateTime IssuedOn { get; set; }
    }

    public class PreprintedsupplierQRDataResponse
    {
        public required int Id { get; set; }
        public required string ReferenceNo { get; set; }
        public required string Type { get; set; }
        public required string Srno { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDesc { get; set; } = string.Empty;

        public required long FromNo { get; set; }
        public required long ToNo { get; set; }
        public required long Qty { get; set; }
        public required bool IsPrinted { get; set; }
        public required string UnitPrefix { get; set; }
        public DateTime IssuedOn { get; set; }
    }

    public class PreprintedQRsq
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDesc { get; set; } = string.Empty;
        public required bool IsExistingStock { get; set; }
        public required string Prefix { get; set; }
        public required string Postfix { get; set; }
        public required string QRURL { get; set; }
        public required string QRPrefix { get; set; }
        public required string QRType { get; set; }
        public List<PreprintedQRDetails> QRs { get; set; } = [];

    }

    public class PreprintedQRReprintResponse
    {
        public required string Prefix { get; set; }
        public required string ItemCode { get; set; }
        public required string ItemDesc { get; set; }
        public required string Srno { get; set; }
        public required string encryptsrno { get; set; }

        
        public required string QRURL { get; set; }
        public required string QRPrefix { get; set; }
        public required string QRType { get; set; }
    }

    public class PreprintedSupplierQRsq
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDesc { get; set; } = string.Empty;
        public required string Prefix { get; set; }
        public required string Postfix { get; set; }
        public required string QRURL { get; set; }
        public required string QRPrefix { get; set; }
        public required string QRType { get; set; }
        public List<PreprintedSupplierQRDetails> QRs { get; set; } = [];

    }

    public class PreprintedSupplierQRDetails
    {
        public required string QRData { get; set; }
        public required bool IsPrinted { get; set; }


    }
    public class PreprintedQR
    {
        public string ItemCode { get; set; } = string.Empty;
        public required bool IsExistingStock { get; set; }
        public required string Prefix { get; set; }
        public required string Postfix { get; set; }
        public required string QRURL { get; set; }
        //  public required string UnitPrefix { get; set; }
        public required string QRPrefix { get; set; }
        public required string QRType { get; set; }
        public List<PreprintedQRDetails> QRs { get; set; } = [];

    }

    public class PreprintedQRDetails
    {
        public long QRseqno { get; set; }
        public required string QRData { get; set; }

    }

    public class PreprintedQRReprintRequest
    {
        public required string QRType { get; set; }
        [JsonIgnore]
        public  int UserId { get; set; }
        public required string Srno { get; set; }
        [JsonIgnore]
        public string Prefix { get; set; }= string.Empty;
    }
    public class QRRequest
    {
        public required int FromNo { get; set; }
        public required int ToNo { get; set; }
        public required string QRType { get; set; }
        public required string UnitPrefix { get; set; }
    }

    public class CurrentStockRequest
    {
        public required string FromDate { get; set; }
        public required string ToDate { get; set; }
    }

    public class PrintedSeqResponse
    {
        public required string QR { get; set; }
        public required string QRType { get; set; }
        public required string ItemCode { get; set; }
        public required string Qty { get; set; }
        public required string PrintedOn { get; set; }
    }

    public class PrintedSeq : PrintedSeqResponse
    {
        [JsonIgnore]
        public required long SeqNo { get; set; }
        [JsonIgnore]
        public required string Prefix { get; set; }
    }
}

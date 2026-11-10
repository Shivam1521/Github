namespace Entities
{
    public class SrUpload
    {
        public string ItemCode { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int QtyToBeDispatched { get; set; }
        public int CreatedBy { get; set; }
        public string LineNumber { get; set; } = string.Empty;
    }

    public class SrUploadResponse : SrUpload
    {
        public int TotalRows { get; set; }
    }
    public class SerialReport
    {
        public string SrNo { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string Lineno { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string ProdLocation { get; set; } = string.Empty;
        public int? QtyToBeDispatched { get; set; }
        public int? FromNoOuter { get; set; }
        public int? ToNoOuter { get; set; }
        public int? FromNoMaster { get; set; }
        public int? ToNoMaster { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Type { get; set; } = string.Empty;
        public int TotalRows { get; set; }

    }

    public class PrintJobReport
    {
        public string SrNo { get; set; } = string.Empty;
        public string JobNo { get; set; } = string.Empty;
        public int? FromNoOuter { get; set; }
        public int? ToNoOuter { get; set; }
        public int? FromNoMaster { get; set; }
        public int? ToNoMaster { get; set; }
        public int? Qty { get; set; }
        public int LastOuterPrinted { get; set; }
        public int LastMasterPrinted { get; set; }
        public DateTime? CreatedOn { get; set; }

    }

}

namespace Entities
{
    public class PrintingDetailsRequest
    {
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public int? LotSizeQty { get; set; }
        public int? NoOfBoxes { get; set; }
        public int? NoOfCards { get; set; }
        public int? QtyToBeDispatched { get; set; }
        public string? Brand { get; set; }
        public string? SubInv { get; set; }
        public string? MSL { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool? IsPrint { get; set; }
        public string? ItemName { get; set; }
        public int PrintSourceId { get; set; }
    }
    public class ItemCodeRequest
    {
        public string? ItemCode { get; set; }
    }
    public class PrintIdListRequest
    {
        public int PrintId { get; set; }
        public int PrintSourceId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public int LotSizeQty { get; set; }
        public int NoOfBoxes { get; set; }
        public int NoOfCards { get; set; }
        public int QtyToBeDispatched { get; set; }
        public int ParcelNumber { get; set; }
        public string? Brand { get; set; }
        public string? SubInv { get; set; }
        public string? MSL { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class SavePrintedDataRequest
    {
        public List<PrintIdListRequest> PrintIds { get; set; } = new();
        public int? PrintedBy { get; set; }
    }
    public class PrintingDetailsResponse
    {
        public int PrintId { get; set; }
        public int? PrintSourceId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public int LotSizeQty { get; set; }
        public int NoOfBoxes { get; set; }
        public int NoOfCards { get; set; }
        public int QtyToBeDispatched { get; set; }
        public string? Brand { get; set; }
        public string? SubInv { get; set; }
        public string? MSL { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class PrintIdRequest
    {
        public int PrintId { get; set; }
    }

    public class MasterPrintingRequest
    {
        public int PrintBy { get; set; }
        public List<PrintIdRequest>? PrintIds { get; set; }
    }

    public class PrintList
    {
        public string SrNo { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string LotSize { get; set; } = string.Empty;
        public string LineNo { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string QtyToBeDispatched { get; set; } = string.Empty;
        public long FromNoOuter { get; set; }
        public long ToNoOuter { get; set; }
        public long FromNoMaster { get; set; }
        public long ToNoMaster { get; set; }
        public string Type { get; set; } = string.Empty;
        public long MasterCartonPackingSize { get; set; }
        public string TotalAssignedQty { get; set; } = string.Empty;
    }
}

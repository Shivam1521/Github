namespace Entities
{
    public class Items
    {
        public required string ItemCode { get; set; }
        public required string Description { get; set; }
        public required string Brand { get; set; } = string.Empty;
        public required string MFGLocation { get; set; }
        public string Class { get; set; } = string.Empty;
        public int? MasterCartonPackingSize { get; set; }
        public int? OuterBoxPackingSize { get; set; }
    }

    public class Plants
    {
        public required string PlantCode { get; set; }
        public required string PlantName { get; set; }
        public required string PlantType { get; set; }
        public required string AddressLine { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Country { get; set; }
        public required string PostalCode { get; set; }
    }

    public class PickSlips
    {
        public required string PickSlipNumber { get; set; }
        public required string SourceBU { get; set; }
        public required string ItemCode { get; set; }
        public required string DeliveryDtlId { get; set; }
        public required string LineStatus { get; set; }
        public required int Quantity { get; set; }
        public string DestinationBU { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public required string CreatedOn { get; set; }
        public string UpdatedOn { get; set; } = string.Empty;
        public string? CHILD_PICKSLIP_NUM { get; set; } = string.Empty;
        public string? PARENT_PICKSLIP_NUM { get; set; } = string.Empty;
    }

    public class MergePickSlips
    {
        public required string DeliveryDtlId { get; set; }
        public string UpdatedOn { get; set; } = string.Empty;
        public string? CHILD_PICKSLIP_NUM { get; set; } = string.Empty;
        public string? PARENT_PICKSLIP_NUM { get; set; } = string.Empty;
    }

    public class Kanban
    {
        public required string MfgLocation { get; set; }
        public required string ItemCode { get; set; }
        public required long LotSize { get; set; }
        public required int CardQty { get; set; }
        public required long QtyToBeDispatched { get; set; }
        public required string Priority { get; set; }
        public required string TxnDateTime { get; set; }
        public required long MSL { get; set; }
        public required long WHOnHand { get; set; }
        public required int CardPicked { get; set; }
        public required long PickedQty { get; set; }
    }
}

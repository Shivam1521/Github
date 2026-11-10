using System.Text.Json.Serialization;

namespace Entities
{
    public class PickSlip
    {
        public required string PickSlipNumber { get; set; }
        public string SourceLocation { get; set; } = string.Empty;
        public string DestinationLocation { get; set; } = string.Empty;
        [JsonIgnore]
        public string Items { get; set; } = string.Empty;
        public List<PickSlipItem> ItemDetails { get; set; } = [];
    }

    public class PickSlipItem
    {
        public required string ItemCode { get; set; }
        public required string DeliveryDtlId { get; set; }
        public required int Quantity { get; set; }
    }


 

    public class Received
    {
        public required string documentNumber { get; set; }
        public string SourceLocation { get; set; } = string.Empty;
        public string SourceCode { get; set; } = string.Empty;
        public string DestinationLocation { get; set; } = string.Empty;
        public string DestinationCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string Items { get; set; } = string.Empty;
        public List<ReceivedItem> ItemDetails { get; set; } = [];
    }
    public class ReceivedItem
    {
        public required string ItemCode { get; set; }
        public required string DeliveryDtlId { get; set; }
        public required int Quantity { get; set; }
    }


    public class PickSlipQRDetails
    {
        public required string PickSlipNumber { get; set; }
        public required string SourceBU { get; set; }
        public required string DestinationBU { get; set; }
        public required string AddedOn { get; set; }
        public List<PickSlipItemDetails> Items { get; set; } = [];
    }

    public class PickSlipItemDetails
    {
        public required string ItemCode { get; set; }
        public required string Quantity { get; set; }
        public required string DeliveryDtlId { get; set; }
        public List<PickSlipQRs> QRs { get; set; } = [];
    }

    public class PickSlipQRs
    {
        [JsonIgnore]
        public long SeqNo { get; set; }
        public long SrNo { get; set; }
        public required string QR { get; set; }
        public required string QRType { get; set; }
        public required string Quantity { get; set; }
        public required string MFGMonth { get; set; }
        public required string SrPrefix { get; set; } = string.Empty;
    }

    public class PickSlipDetails
    {
        public required string PickSlipNumber { get; set; }
        public required string SourceBU { get; set; }
        [JsonIgnore]
        public string Prefix { get; set; } = string.Empty;
        public required string ItemCode { get; set; }
        public required string DeliveryDtlId { get; set; }
        public required string Quantity { get; set; }
        public required string DestinationBU { get; set; }
        public required string AddedOn { get; set; }
        [JsonIgnore]
        public string SeqNos { get; set; } = string.Empty;
        public List<PickSlipQRs> QRs { get; set; } = [];
    }

    public class QRDetailsRequest
    {
        public required long SeqNo { get; set; }
        public required string QRType { get; set; }
        public required string Prefix { get; set; }
    }

    public class PickupQRDetailsRequestDB : QRDetailsRequest
    {
        public required string PickSlipNumber { get; set; }
        public int Loose { get; set; } 
    }

    public class PickupQRDetailsRequest
    {
        public required string PickSlipNumber { get; set; }
        public required string QR { get; set; }
        public int Loose { get; set; } = 0;
    }

    public class ReceiveCartonResponse : RecivedQRDetails
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;

    }

    public class RecivedQRDetails
    {
        public required long SeqNo { get; set; }
        public string QRType { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class PickupQRDetailsResponse : PickupQRDetails
    {
        [JsonIgnore]
        public string ResponseCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string ResponseMessage { get; set; } = string.Empty;
    }

    public class PickupQRDetails
    {
        public required long SeqNo { get; set; }
        public string QRType { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class PickSlipDetailsDto
    {
        public required long SeqNo { get; set; }
        public required string QRType { get; set; }
        public required int UnitId { get; set; }
        public required string ItemCode { get; set; } = string.Empty;
        public required string DeliveryDtlId { get; set; } = string.Empty;
        public required int Quantity { get; set; }
    }

    public class InsertPickSlipRequest
    {
        public required string PickSlipNumber { get; set; } = string.Empty;
        public int AddedBy { get; set; }
        public List<PickSlipDetailsDto> PickSlipDetails { get; set; } = [];
    }

    public class PickSlipReportDetails
    {
        public string PickSlipNumber { get; set; } = string.Empty;
        public string SourceBU { get; set; } = string.Empty;
        public string DestinationBU { get; set; } = string.Empty;
        public string DeliveryDtlId { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PickedQuantity { get; set; }
        public long SeqNo { get; set; }
        public string QR { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public int ItemQty { get; set; }
        public string QRType { get; set; } = string.Empty;
        public string PrintedAt { get; set; } = string.Empty;
        public string PickedOn { get; set; } = string.Empty;
        [JsonIgnore]
        public string Prefix { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }

    public class PickSlipReceiptStatus
    {
        public string PickSlipNumber { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string DeliveryDetailId { get; set; } = string.Empty;
        public long SeqNo { get; set; }
        public string Location { get; set; } = string.Empty;
        [JsonIgnore]
        public string Prefix { get; set; } = string.Empty;
        public string QRType { get; set; } = string.Empty;
        public string QR { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int UnitId { get; set; }
    }

    public class PickSlipEBSRequest
    {
        public List<PickSlipEBSDetail> Pickslip_detail { get; set; } = [];
    }

    public class PickSlipEBSDetail
    {
        public required string pickslip_number { get; set; }
        public required string item_code { get; set; }
        public required int DELIVERY_DETAIL_ID { get; set; }
        public required int SMART_SCAN_QTY { get; set; }
    }



    public class DemapPickSlipDetailRequest
    {
        public string QR { get; set; }
    }

    public class DemapPickSlipDetailRequestDB
    {
        public long SeqNo { get; set; }
        public string Prefix { get; set; }
        public string QRType { get; set; }
    }

    public class DemapPickSlipDetailResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class PendingPickslipResponse
    {
        public string PickslipNumber { get; set; }
        public int ScannedCount { get; set; }
        public DateTime LastScannedOn { get; set; }
    }

    public class BulkSubmitPickSlipsRequest
    {
        public List<string> PickSlipNumbers { get; set; }
    }

   
    public class BulkSubmitPickSlipsRequestDB
    {
        public List<string> PickSlipNumbers { get; set; }
        public int UserId { get; set; }
    }

    public class BulkSubmitPickSlipResult
    {
        public string PickSlipNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}

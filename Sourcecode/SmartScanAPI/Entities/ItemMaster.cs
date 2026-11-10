using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Entities
{
    public class ItemDDL
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class ItemMaster
    {
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string MfgLocation { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Masterqty { get; set; }
        public int? MasterCartonPackingSize { get; set; }
        public int? OuterBoxPackingSize { get; set; }
        public int? OuterPerCarton { get; set; }
        public string LabelPerLot { get; set; } = string.Empty;
        public string Pic1LineSupervisor { get; set; } = string.Empty;
        public string Pic2Engineer { get; set; } = string.Empty;
        public int? LotQty { get; set; }
        public int? TotalCards { get; set; }
        public int? TotalRooms { get; set; }
        public int? CartonPerOuterBoxInLot { get; set; }
        public int WHZoneID { get; set; }
        public required List<Line> Lines { get; set; }
        public string PlaceSerial { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ProductionZone { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool Samriddhi { get; set; }
        public bool SmartSaver { get; set; }
        public int? UnitId { get; set; }
        public int Id { get; set; }
        [JsonIgnore]
        public decimal OuterBoxMinWeight { get; set; }
        [JsonIgnore]
        public decimal OuterBoxMaxWeight { get; set; }

        
        public int Partqty { get; set; }
    }

    public class ItemMasterRequest
    {
        public required int Id { get; set; }
        public int? OuterPerCarton { get; set; }
        public string LabelPerLot { get; set; } = string.Empty;
        public string Pic1LineSupervisor { get; set; } = string.Empty;
        public string Pic2Engineer { get; set; } = string.Empty;
        public int? LotQty { get; set; }
        public int? TotalCards { get; set; }
        public int? TotalRooms { get; set; }
        public int? CartonPerOuterBoxInLot { get; set; }
        public int? WHZoneID { get; set; }
        public List<Line> Lines { get; set; } = [];
        public string PlaceSerial { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ProductionZone { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool Samriddhi { get; set; }
        public bool SmartSaver { get; set; }
        public decimal[] OuterBoxWeightRange { get; set; } = [];
        [JsonIgnore]
        public decimal OuterBoxMinWeight { get; set; }
        [JsonIgnore]
        public decimal OuterBoxMaxWeight { get; set; }
        public int? MasterCartonPackingSize { get; set; }
        public int? OuterBoxPackingSize { get; set; }
    }

    public class ItemDetailsResponse : ItemMaster
    {
        public List<LineDetails> LineDetails { get; set; } = [];
        [JsonIgnore]
        public string Line { get; set; } = string.Empty;
    }

    public class ItemMasterResponse : ItemMaster
    {
        public int TotalRows { get; set; }
        [JsonIgnore]
        public string Line { get; set; } = string.Empty;
        public string WHZone { get; set; } = string.Empty;
        public decimal[] OuterBoxWeightRange { get; set; } = [];
    }

    public class ItemUserImportRow
    {
        public required string ItemCode { get; set; }
        public required string ItemDescription { get; set; }
        public required string Brand { get; set; }
        public required string MfgLocation { get; set; }
        public required string Class { get; set; }
        public required int MasterPackingSize { get; set; }
        public required int OuterPackingSize { get; set; }
        public string Supervisor { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LineNo { get; set; } = string.Empty;
        public string FanType { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public int Labels { get; set; } 
    }

    public class MapItemsAndUsersRequest
    {
        public required IFormFile File { get; set; }
    }
}

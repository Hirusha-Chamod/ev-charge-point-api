
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ev_charge_point_api.Models
{
    public class ChargingStation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("location")]
        public Location Location { get; set; } = new Location();

        [BsonElement("type")]
        public string Type { get; set; } = "AC";

        [BsonElement("slots")]
        public List<Slot> Slots { get; set; } = new List<Slot>();

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
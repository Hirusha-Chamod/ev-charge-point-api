
using MongoDB.Bson.Serialization.Attributes;

namespace ev_charge_point_api.Models
{
    public class Location
    {
        [BsonElement("type")]
        public string Type { get; set; } = "Point"; // GeoJSON type

        [BsonElement("coordinates")]
        public double[] Coordinates { get; set; }
    }
}
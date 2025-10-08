// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>Location.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-06</date>
// <summary>
//   Defines the data model for a GeoJSON Point, used for storing geographic coordinates in MongoDB.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
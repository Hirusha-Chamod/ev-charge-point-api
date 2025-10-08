// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>Booking.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the data model for a Booking.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ev_charge_point_api.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // EV Owner NIC
        [BsonElement("evOwnerNic")]
        public string EvOwnerNic { get; set; } = string.Empty;

        // Charging Station reference
        [BsonElement("stationId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; } = string.Empty;

        // SlotId from ChargingStation.Slots
        [BsonElement("slotId")]
        public int SlotId { get; set; }

        // Reservation date/time
        [BsonElement("reservationDateTime")]
        public DateTime ReservationDateTime { get; set; }

        // Status of bookin.
        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }

}

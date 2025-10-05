// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>EvUser.cs</file>
// <author>Thilochana J M (IT22899224)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Model representing an EV user entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;

namespace ev_charge_point_api.Models
{
    // EV Owner specific user entity. Uses NIC as primary key.
    public class EvUser
    {
        [BsonId]
        public string Nic { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

    // True when account is active. Owners can deactivate their own accounts; only backoffice can reactivate.
    public bool IsActive { get; set; } = true;

    // Optional reason stored when an account is deactivated.
    public string? DeactivationReason { get; set; }

    public string? DeactivatedBy { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    }
}

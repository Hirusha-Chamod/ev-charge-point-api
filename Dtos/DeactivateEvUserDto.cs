// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>DeactivateEvUserDto.cs</file>
// <author>Thilochana J M (IT22899224)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Data transfer object for deactivating an EV user account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class DeactivateEvUserDto
    {
        [Required]
        public string Nic { get; set; }

        public string? Reason { get; set; }
    }
}
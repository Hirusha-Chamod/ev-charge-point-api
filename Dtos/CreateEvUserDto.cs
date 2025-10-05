// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>CreateEvUserDto.cs</file>
// <author>Thilochana J M (IT22899224)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Data transfer object for creating a new EV user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class CreateEvUserDto
    {
        [Required]
        public string Nic { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
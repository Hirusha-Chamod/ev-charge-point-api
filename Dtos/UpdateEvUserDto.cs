using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class UpdateEvUserDto
    {
        [Required]
        public string Nic { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Optional - if provided, password will be changed
        public string? Password { get; set; }
    }
}
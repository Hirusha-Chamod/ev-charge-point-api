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
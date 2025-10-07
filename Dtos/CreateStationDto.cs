
using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class CreateStationDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "AC"; // "AC" or "DC"

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        public int NumberOfSlots { get; set; } = 1;
    }
}
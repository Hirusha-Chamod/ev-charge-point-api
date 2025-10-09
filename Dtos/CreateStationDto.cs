// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>CreateStationDto.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-07</date>
// <summary>
//  Defines the data transfer object for creating a new charging station.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


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
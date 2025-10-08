// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>UpdateStationDto.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the data transfer object for updating an existing charging station.
//   Properties are nullable to support partial updates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class UpdateStationDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

        public string? Type { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }

        [Range(1, 16)]
        public int? NumberOfSlots { get; set; }
    }
}
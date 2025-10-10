// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>UpdateSlotStatusDto.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-10</date>
// <summary>
//   Defines the data transfer object for updating the IsAvailable status of a single slot.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ev_charge_point_api.Dtos
{
    public class UpdateSlotStatusDto
    {
        public bool IsAvailable { get; set; }
    }
}
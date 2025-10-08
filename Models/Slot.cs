// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>Slot.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-06</date>
// <summary>
//   Defines the data model for a single charging slot within a Charging Station.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ev_charge_point_api.Models
{
    public class Slot
    {
        public int SlotId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
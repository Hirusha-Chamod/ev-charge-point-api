// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the data transfer object for creating a new booking,
//   encapsulating necessary information for the operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class CreateBookingDto
    {
        [Required]
        public string EvOwnerNic { get; set; } = string.Empty;

        [Required]
        public string StationId { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int SlotId { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }
    }
}

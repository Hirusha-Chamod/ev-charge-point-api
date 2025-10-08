// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the data transfer object for cancelling a booking,
//   encapsulating necessary information for the operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class CancelBookingDto
    {
        // EV owner's NIC is required when the caller is not a privileged role
        public string? EvOwnerNic { get; set; }
    }
}

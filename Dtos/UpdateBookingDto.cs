// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the data transfer object for updating a booking,
//   encapsulating fields that can be modified.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using ev_charge_point_api.Models;

namespace ev_charge_point_api.Dtos
{
    public class UpdateBookingDto
    {
        public BookingStatus? Status { get; set; }

        public DateTime? BookingDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}

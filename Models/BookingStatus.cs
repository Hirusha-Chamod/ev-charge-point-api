// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingStatus.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the BookingStatus enumeration representing possible states of a booking.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ev_charge_point_api.Models
{
    public enum BookingStatus
    {
        Pending,
        Approved,
        Cancelled,
        Completed,
        Arrived,
    }
}

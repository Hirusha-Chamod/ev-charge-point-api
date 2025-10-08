// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>IBookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the interfaces for BookingService, outlining methods for managing bookings.
// </summary>

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;

namespace ev_charge_point_api.Services
{
    public interface IBookingService
    {
        // Retrieves all bookings
        Task<IEnumerable<Booking>> GetAllBookingsAsync();

        // Retrieves booking by ID
        Task<Booking?> GetBookingByIdAsync(string id);

        // Retrieves all bookings for a specific EV Owner
        Task<IEnumerable<Booking>> GetBookingsByOwnerAsync(string evOwnerNic);

        // Creates a new booking
        Task<Booking?> CreateBookingAsync(CreateBookingDto createDto);

        // Updates an existing booking
        Task<bool> UpdateBookingAsync(string id, UpdateBookingDto updateDto);

        // Cancels an existing booking
        Task<bool> CancelBookingAsync(string id);

        // Updates only booking status (Pending, Approved, Cancelled, Completed)
        Task<bool> UpdateBookingStatusAsync(string id, BookingStatus newStatus);
    }
}

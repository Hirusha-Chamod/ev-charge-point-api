// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Defines the contract for Booking data access operations,
//   abstracting the underlying database interactions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Models;

namespace ev_charge_point_api.Repositories
{
    public interface IBookingRepository
    {
        // Creates a new booking document.
        Task<Booking> CreateAsync(Booking booking);
        // Retrieves a booking document by its unique ID.
        Task<Booking?> GetByIdAsync(string id);
        // Retrieves all booking documents.
        Task<IEnumerable<Booking>> GetAllAsync();
        // Updates an existing booking document.
        Task<bool> UpdateAsync(Booking booking);
        // Updates only the status of a booking document.
        Task<bool> UpdateStatusAsync(string id, BookingStatus newStatus);


        // get bookings for a specific EV owner.
        Task<IEnumerable<Booking>> GetByOwnerNicAsync(string evOwnerNic);
        // get active bookings for a specific charging station.
        Task<IEnumerable<Booking>> GetActiveBookingsByStationAsync(string stationId);
        // Deactivates a booking by setting its IsActive flag to false.
        Task<bool> DeactivateAsync(string id);
    }
}
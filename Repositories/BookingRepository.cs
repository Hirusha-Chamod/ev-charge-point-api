// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Implements the data access layer for Bookings, interacting with MongoDB
//   to perform CRUD operations and complex queries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Driver;

namespace ev_charge_point_api.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IMongoCollection<Booking> _bookingsCollection;

        public BookingRepository(MongoDBService mongoDBService)
        {
            _bookingsCollection = mongoDBService.GetCollection<Booking>("bookings");
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.IsActive = true;
            await _bookingsCollection.InsertOneAsync(booking);
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _bookingsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(string id)
        {
            return await _bookingsCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Booking>> GetByOwnerNicAsync(string evOwnerNic)
        {
            return await _bookingsCollection.Find(b => b.EvOwnerNic == evOwnerNic).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Booking booking)
        {
            booking.UpdatedAt = DateTime.UtcNow;
            var result = await _bookingsCollection.ReplaceOneAsync(b => b.Id == booking.Id, booking);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Updates only the booking status (and updatedAt timestamp)
        public async Task<bool> UpdateStatusAsync(string id, BookingStatus newStatus)
        {
            var filter = Builders<Booking>.Filter.Eq(b => b.Id, id);
            var update = Builders<Booking>.Update
                .Set(b => b.Status, newStatus)
                .Set(b => b.UpdatedAt, DateTime.UtcNow);

            var result = await _bookingsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Booking>> GetActiveBookingsByStationAsync(string stationId)
        {
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.StationId, stationId),
                Builders<Booking>.Filter.Ne(b => b.Status, BookingStatus.Cancelled),
                Builders<Booking>.Filter.Gte(b => b.ReservationDateTime, DateTime.UtcNow)
            );

            return await _bookingsCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var filter = Builders<Booking>.Filter.Eq(b => b.Id, id);
            var update = Builders<Booking>.Update.Set(b => b.IsActive, false).Set(b => b.UpdatedAt, DateTime.UtcNow);
            var result = await _bookingsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}

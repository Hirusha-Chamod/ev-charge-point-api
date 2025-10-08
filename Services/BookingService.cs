// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>BookingService.cs</file>
// <author>Senanayake S.M.A.S.N (IT22305282)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Implements the business logic for managing Bookings, acting as a mediator
//   between the controller and the repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;
using System.Linq;

namespace ev_charge_point_api.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _stationRepository;

        // Initializes a new instance of the BookingService class.
        public BookingService(IBookingRepository bookingRepository, IChargingStationRepository stationRepository)
        {
            _bookingRepository = bookingRepository;
            _stationRepository = stationRepository;
        }

        // get all bookings
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        // get booking by ID
        public async Task<Booking?> GetBookingByIdAsync(string id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        // get bookings by EV Owner NIC
        public async Task<IEnumerable<Booking>> GetBookingsByOwnerAsync(string evOwnerNic)
        {
            return await _bookingRepository.GetByOwnerNicAsync(evOwnerNic);
        }

        // Creates a new booking
        public async Task<Booking?> CreateBookingAsync(CreateBookingDto createDto)
        {
            // Check station exists and is active
            var station = await _stationRepository.GetByIdAsync(createDto.StationId);
            if (station == null || !station.IsActive)
                throw new InvalidOperationException("Station not found or inactive.");

            // apply 7-day rule
            var now = DateTime.UtcNow;
            var diff = (createDto.ReservationDateTime - now).TotalDays;
            if (diff > 7 || diff < 0)
                throw new InvalidOperationException("Reservation must be within 7 days from now.");

            // Ensure slot exists
            var slotExists = station.Slots != null && station.Slots.Any(s => s.SlotId == createDto.SlotId);
            if (!slotExists)
                throw new InvalidOperationException("Invalid slot selected.");

            var booking = new Booking
            {
                EvOwnerNic = createDto.EvOwnerNic,
                StationId = createDto.StationId,
                SlotId = createDto.SlotId,
                ReservationDateTime = createDto.ReservationDateTime,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _bookingRepository.CreateAsync(booking);
            return booking;
        }

        public async Task<bool> UpdateBookingAsync(string id, UpdateBookingDto updateDto)
        {
            var existing = await _bookingRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Booking not found.");

            // apply 12-hour rule for time or status changes
            var hoursUntilReservation = (existing.ReservationDateTime - DateTime.UtcNow).TotalHours;
            if (hoursUntilReservation < 12 && updateDto.Status != BookingStatus.Completed)
                throw new InvalidOperationException("Bookings can only be updated at least 12 hours before the reservation time.");

            // If a new date/time is provided, apply it
            if (updateDto.ReservationDateTime.HasValue)
            {
                var newDate = updateDto.ReservationDateTime.Value;
                var daysAhead = (newDate - DateTime.UtcNow).TotalDays;
                if (daysAhead > 7 || daysAhead < 0)
                    throw new InvalidOperationException("Reservation date must be within 7 days from now.");

                existing.ReservationDateTime = newDate;
            }

            // If a status update is provided, apply it
            if (updateDto.Status.HasValue)
            {
                existing.Status = updateDto.Status.Value;
            }

            // Refresh update timestamp
            existing.UpdatedAt = DateTime.UtcNow;

            // Save changes to DB
            return await _bookingRepository.UpdateAsync(existing);
        }


        // Cancels a booking
        public async Task<bool> CancelBookingAsync(string id)
        {
            var existing = await _bookingRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Booking not found.");

            // apply 12-hour rule for cancellation
            var hoursUntilReservation = (existing.ReservationDateTime - DateTime.UtcNow).TotalHours;
            if (hoursUntilReservation < 12)
                throw new InvalidOperationException("Bookings can only be cancelled at least 12 hours before reservation.");

            return await _bookingRepository.UpdateStatusAsync(id, BookingStatus.Cancelled);
        }

        // Updates booking status directly
        public async Task<bool> UpdateBookingStatusAsync(string id, BookingStatus newStatus)
        {
            return await _bookingRepository.UpdateStatusAsync(id, newStatus);
        }
    }
}

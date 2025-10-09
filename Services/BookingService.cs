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

            // Validate booking date is not in the past
            var bookingDateOnly = createDto.BookingDate.Date;
            var today = DateTime.UtcNow.Date;
            if (bookingDateOnly < today)
                throw new InvalidOperationException("Booking date cannot be in the past.");

            // apply 7-day rule for booking date
            var diff = (bookingDateOnly - today).TotalDays;
            if (diff > 7)
                throw new InvalidOperationException("Reservation must be within 7 days from now.");

            // Validate start time is before end time
            if (createDto.StartTime >= createDto.EndTime)
                throw new InvalidOperationException("Start time must be before end time.");

            // Ensure booking duration is reasonable (e.g., max 24 hours)
            var duration = (createDto.EndTime - createDto.StartTime).TotalHours;
            if (duration > 24)
                throw new InvalidOperationException("Booking duration cannot exceed 24 hours.");

            // Ensure slot exists
            var slotExists = station.Slots != null && station.Slots.Any(s => s.SlotId == createDto.SlotId);
            if (!slotExists)
                throw new InvalidOperationException("Invalid slot selected.");

            // Check for overlapping bookings at the same slot on the same date (prevent double-booking)
            var existingBookings = await _bookingRepository.GetAllAsync();
            var conflictingBooking = existingBookings.FirstOrDefault(b => 
                b.StationId == createDto.StationId && 
                b.SlotId == createDto.SlotId && 
                b.BookingDate.Date == createDto.BookingDate.Date &&
                b.Status != BookingStatus.Cancelled &&
                b.IsActive &&
                // Check for time overlap: new booking overlaps if it starts before existing ends and ends after existing starts
                createDto.StartTime < b.EndTime && createDto.EndTime > b.StartTime);

            if (conflictingBooking != null)
                throw new InvalidOperationException("This slot is already booked during the selected time period on this date.");

            var booking = new Booking
            {
                EvOwnerNic = createDto.EvOwnerNic,
                StationId = createDto.StationId,
                SlotId = createDto.SlotId,
                BookingDate = createDto.BookingDate,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
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
            var bookingDateTime = existing.BookingDate.Date.Add(existing.StartTime.TimeOfDay);
            var hoursUntilReservation = (bookingDateTime - DateTime.UtcNow).TotalHours;
            if (hoursUntilReservation < 12 && updateDto.Status != BookingStatus.Completed)
                throw new InvalidOperationException("Bookings can only be updated at least 12 hours before the reservation time.");

            // If new date/times are provided, apply them
            if (updateDto.BookingDate.HasValue || updateDto.StartTime.HasValue || updateDto.EndTime.HasValue)
            {
                var newBookingDate = updateDto.BookingDate ?? existing.BookingDate;
                var newStartTime = updateDto.StartTime ?? existing.StartTime;
                var newEndTime = updateDto.EndTime ?? existing.EndTime;

                // Validate booking date is not in the past
                var bookingDateOnly = newBookingDate.Date;
                var today = DateTime.UtcNow.Date;
                if (bookingDateOnly < today)
                    throw new InvalidOperationException("Booking date cannot be in the past.");

                // apply 7-day rule for booking date
                var diff = (bookingDateOnly - today).TotalDays;
                if (diff > 7)
                    throw new InvalidOperationException("Reservation must be within 7 days from now.");

                // Validate start time is before end time
                if (newStartTime >= newEndTime)
                    throw new InvalidOperationException("Start time must be before end time.");

                // Ensure booking duration is reasonable (e.g., max 24 hours)
                var duration = (newEndTime - newStartTime).TotalHours;
                if (duration > 24)
                    throw new InvalidOperationException("Booking duration cannot exceed 24 hours.");

                // Check for conflicts when updating reservation time
                var allBookings = await _bookingRepository.GetAllAsync();
                var conflictingBooking = allBookings.FirstOrDefault(b => 
                    b.Id != existing.Id && // Exclude current booking
                    b.StationId == existing.StationId && 
                    b.SlotId == existing.SlotId && 
                    b.BookingDate.Date == newBookingDate.Date &&
                    b.Status != BookingStatus.Cancelled &&
                    b.IsActive &&
                    // Check for time overlap
                    newStartTime < b.EndTime && newEndTime > b.StartTime);

                if (conflictingBooking != null)
                    throw new InvalidOperationException("This slot is already booked during the selected time period on this date.");

                existing.BookingDate = newBookingDate;
                existing.StartTime = newStartTime;
                existing.EndTime = newEndTime;
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
            var bookingDateTime = existing.BookingDate.Date.Add(existing.StartTime.TimeOfDay);
            var hoursUntilReservation = (bookingDateTime - DateTime.UtcNow).TotalHours;
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

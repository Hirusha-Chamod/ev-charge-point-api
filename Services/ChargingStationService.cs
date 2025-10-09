// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>ChargingStationService.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-07</date>
// <summary>
//   Implements the business logic for managing Charging Stations, acting as the mediator
//   between the controller and the repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;

namespace ev_charge_point_api.Services
{
    public class ChargingStationService : IChargingStationService
    {
        private readonly IChargingStationRepository _stationRepository;
        private readonly IBookingRepository _bookingRepository;

        // Initializes a new instance of the ChargingStationService class.
        public ChargingStationService(IChargingStationRepository stationRepository, IBookingRepository bookingRepository)
        {
            _stationRepository = stationRepository;
            _bookingRepository = bookingRepository;

        }

        // Creates a new charging station based on the provided DTO data.
        public async Task<ChargingStation> CreateStationAsync(CreateStationDto createDto)
        {
            var station = new ChargingStation
            {
                Name = createDto.Name,
                Type = createDto.Type,
                Location = new Location
                {
                    Coordinates = new double[] { createDto.Longitude, createDto.Latitude }
                },
                IsActive = true,
                Slots = Enumerable.Range(1, createDto.NumberOfSlots)
                                  .Select(id => new Slot { SlotId = id, IsAvailable = true })
                                  .ToList()
            };

            await _stationRepository.CreateAsync(station);
            return station;
        }

        // Deactivates a specific charging station.
        public async Task<bool> DeactivateStationAsync(string id)
        {

            var activeBookings = await _bookingRepository.GetActiveBookingsByStationAsync(id);
            if (activeBookings.Any())
            {
                // Rule violated: active bookings exist, so return failure.
                return false;
            }
            return await _stationRepository.DeactivateAsync(id);
        }

        // Activates a specific charging station.
        public async Task<bool> ActivateStationAsync(string id)
        {
            var stationToActivate = await _stationRepository.GetByIdAsync(id);
            if (stationToActivate is null)
            {
                return false; // The station doesn't exist, so we can't activate it.
            }

            if (stationToActivate.IsActive)
            {
                return true;
            }

            return await _stationRepository.ActivateAsync(id);
        }

        // Retrieves all charging stations.
        public async Task<IEnumerable<ChargingStation>> GetAllStationsAsync()
        {
            return await _stationRepository.GetAllAsync();
        }

        // Retrieves a single charging station by its unique ID.
        public async Task<ChargingStation?> GetStationByIdAsync(string id)
        {
            return await _stationRepository.GetByIdAsync(id);
        }

        // Updates an existing charging station.
        public async Task<bool> UpdateStationAsync(string id, UpdateStationDto updateDto)
        {
            var stationToUpdate = await _stationRepository.GetByIdAsync(id);
            if (stationToUpdate is null)
            {
                return false;
            }

            // Apply changes from the DTO only if the properties are not null
            if (updateDto.Name is not null)
            {
                stationToUpdate.Name = updateDto.Name;
            }
            if (updateDto.Type is not null)
            {
                stationToUpdate.Type = updateDto.Type;
            }

            if (updateDto.Latitude.HasValue && updateDto.Longitude.HasValue)
            {
                stationToUpdate.Location.Coordinates[1] = updateDto.Latitude.Value;
                stationToUpdate.Location.Coordinates[0] = updateDto.Longitude.Value;
            }


            return await _stationRepository.UpdateAsync(stationToUpdate);
        }

        // Finds all charging stations near a specified geographic location.
        public async Task<IEnumerable<ChargingStation>> GetNearbyStationsAsync(double longitude, double latitude)
        {
            return await _stationRepository.GetNearbyAsync(longitude, latitude);
        }

        // Gets available slots for a specific station at a desired time.
        public async Task<IEnumerable<int>> GetAvailableSlotsAsync(string stationId, DateTime desiredStartTime, DateTime desiredEndTime)
        {
            var station = await _stationRepository.GetByIdAsync(stationId);
            if (station == null || station.Slots == null)
            {
                return Enumerable.Empty<int>();
            }
            var allSlotIds = station.Slots.Select(s => s.SlotId).ToHashSet();


            var desiredStartTimeUtc = desiredStartTime.ToUniversalTime();
            var desiredEndTimeUtc = desiredEndTime.ToUniversalTime();

            var futureBookings = await _bookingRepository.GetActiveBookingsByStationAsync(stationId);

            var unavailableSlotIds = futureBookings
                .Where(b =>
                {

                    var existingStartTimeUtc = b.StartTime.ToUniversalTime();
                    var existingEndTimeUtc = b.EndTime.ToUniversalTime();

                    return existingStartTimeUtc < desiredEndTimeUtc && existingEndTimeUtc > desiredStartTimeUtc;
                })
                .Select(b => b.SlotId)
                .ToHashSet();

            return allSlotIds.Where(id => !unavailableSlotIds.Contains(id));
        }
    }
}
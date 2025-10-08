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

        // Initializes a new instance of the ChargingStationService class.
        public ChargingStationService(IChargingStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
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

            return await _stationRepository.DeactivateAsync(id);
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

        // Finds all charging stations near a specified geographic location.
        public async Task<IEnumerable<ChargingStation>> GetNearbyStationsAsync(double longitude, double latitude)
        {
            return await _stationRepository.GetNearbyAsync(longitude, latitude);
        }
    }
}
using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;

namespace ev_charge_point_api.Services
{
    public class ChargingStationService : IChargingStationService
    {
        private readonly IChargingStationRepository _stationRepository;

        public ChargingStationService(IChargingStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

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

        public async Task<bool> DeactivateStationAsync(string id)
        {
            return await _stationRepository.DeactivateAsync(id);
        }

        public async Task<IEnumerable<ChargingStation>> GetAllStationsAsync()
        {
            return await _stationRepository.GetAllAsync();
        }

        public async Task<ChargingStation?> GetStationByIdAsync(string id)
        {
            return await _stationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ChargingStation>> GetNearbyStationsAsync(double longitude, double latitude)
        {
            return await _stationRepository.GetNearbyAsync(longitude, latitude);
        }
    }
}
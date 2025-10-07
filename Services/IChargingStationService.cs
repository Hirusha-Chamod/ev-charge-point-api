using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models; 

namespace ev_charge_point_api.Services
{
    public interface IChargingStationService
    {
        Task<IEnumerable<ChargingStation>> GetAllStationsAsync();

        Task<ChargingStation?> GetStationByIdAsync(string id);

        Task<IEnumerable<ChargingStation>> GetNearbyStationsAsync(double longitude, double latitude);

        Task<ChargingStation> CreateStationAsync(CreateStationDto createDto);

        Task<bool> DeactivateStationAsync(string id);
    }
}
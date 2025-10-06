using ev_charge_point_api.Models;

namespace ev_charge_point_api.Repositories
{
    public interface IChargingStationRepository
    {
        Task<ChargingStation?> GetByIdAsync(string id);
        Task<IEnumerable<ChargingStation>> GetAllAsync();
        Task<IEnumerable<ChargingStation>> GetNearbyAsync(double longitude, double latitude, int maxDistanceInMeters = 5000);
        Task CreateAsync(ChargingStation station);
        Task<bool> UpdateAsync(ChargingStation station);
        Task<bool> DeactivateAsync(string id);
    }
}
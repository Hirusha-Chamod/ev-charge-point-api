// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>IChargingStationRepository.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-06</date>
// <summary>
//   Defines the contract for the data access layer for ChargingStation entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Models;

namespace ev_charge_point_api.Repositories
{
    public interface IChargingStationRepository
    {
        // Retrieves a single charging station by its unique ID.
        Task<ChargingStation?> GetByIdAsync(string id);

        // Retrieves all charging stations from the database.
        Task<IEnumerable<ChargingStation>> GetAllAsync();

        // Finds all charging stations within a specified distance of a geographic point.
        Task<IEnumerable<ChargingStation>> GetNearbyAsync(double longitude, double latitude, int maxDistanceInMeters = 5000);

        // Adds a new charging station to the database.
        Task CreateAsync(ChargingStation station);

        // Updates an existing charging station in the database.
        Task<bool> UpdateAsync(ChargingStation station);

        // Marks a charging station as inactive in the database.
        Task<bool> DeactivateAsync(string id);

        // Marks a charging station as active in the database.
        Task<bool> ActivateAsync(string id);

        // Updates the IsAvailable status of a single slot within a station.
        Task<bool> UpdateSlotStatusAsync(string stationId, int slotId, bool isAvailable);
    }
}
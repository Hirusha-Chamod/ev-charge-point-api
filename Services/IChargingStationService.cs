// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>IChargingStationService.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-07</date>
// <summary>
//   Defines the contract for the business logic layer for managing Charging Stations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;

namespace ev_charge_point_api.Services
{
    public interface IChargingStationService
    {
        // Gets all charging stations.
        Task<IEnumerable<ChargingStation>> GetAllStationsAsync();

        // Gets a single charging station by its unique ID.
        Task<ChargingStation?> GetStationByIdAsync(string id);

        // Finds charging stations near a given geographic point.
        Task<IEnumerable<ChargingStation>> GetNearbyStationsAsync(double longitude, double latitude);

        // Creates a new charging station based on the provided data.
        Task<ChargingStation> CreateStationAsync(CreateStationDto createDto);

        // Updates an existing charging station.
        Task<bool> UpdateStationAsync(string id, UpdateStationDto updateDto);

        // Deactivates a charging station, subject to business rules.
        Task<bool> DeactivateStationAsync(string id);

        // Activates a charging station.
        Task<bool> ActivateStationAsync(string id);

        // Gets available slots for a specific station at a desired time.
        Task<IEnumerable<int>> GetAvailableSlotsAsync(string stationId, DateTime desiredStartTime, DateTime desiredEndTime);

        // Updates the availability of a specific slot.
        Task<bool> UpdateSlotAvailabilityAsync(string stationId, int slotId, bool isAvailable);
    }
}
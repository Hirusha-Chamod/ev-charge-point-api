// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>ChargingStationsController.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Exposes API endpoints for managing Charging Stations. All endpoints require authentication,
//   with specific actions restricted to authorized roles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ev_charge_point_api.Controllers
{
    [ApiController]
    [Route("api/stations")]
     // All endpoints require authentication by default
    public class ChargingStationsController : ControllerBase
    {
        private readonly IChargingStationService _stationService;

        // Initializes a new instance of the ChargingStationsController class.
        public ChargingStationsController(IChargingStationService stationService)
        {
            _stationService = stationService;
        }

        // Retrieves all charging stations. Inherits the class-level [Authorize], so any logged-in user can access.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stations = await _stationService.GetAllStationsAsync();
            return Ok(stations);
        }

        // Retrieves a single station by its ID. Any logged in user can access.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var station = await _stationService.GetStationByIdAsync(id);
            if (station is null)
            {
                return NotFound();
            }
            return Ok(station);
        }

        // Retrieves nearby stations based on geographic coordinates. Any logged in user can access.
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double longitude, [FromQuery] double latitude)
        {
            var stations = await _stationService.GetNearbyStationsAsync(longitude, latitude);
            return Ok(stations);
        }

        // Creates a new station
        [HttpPost]
        
        public async Task<IActionResult> Create([FromBody] CreateStationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newStation = await _stationService.CreateStationAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = newStation.Id }, newStation);
        }

        // Updates an existing station
        [HttpPut("{id}")]
        
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStationDto updateDto)
        {
            var success = await _stationService.UpdateStationAsync(id, updateDto);

            if (!success)
            {
                return NotFound("Station with the provided ID not found.");
            }

            return NoContent();
        }

        // Deactivates a station
        [HttpPatch("{id}/deactivate")]
        
        public async Task<IActionResult> Deactivate(string id)
        {
            var success = await _stationService.DeactivateStationAsync(id);
            if (!success)
            {
                return BadRequest("Unable to deactivate station. It may have active bookings or does not exist.");
            }
            return NoContent();
        }

        // Activates a station
        [HttpPatch("{id}/activate")]
        
        public async Task<IActionResult> Activate(string id)
        {
            var success = await _stationService.ActivateStationAsync(id);
            if (!success)
            {
                return NotFound("Station with the provided ID not found.");
            }
            return NoContent();
        }

        // Gets available slots for a station at a specific time.
        [HttpGet("{id}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(string id, [FromQuery] DateTime desiredStartTime, [FromQuery] DateTime desiredEndTime)
        {
            if (desiredStartTime == default || desiredEndTime == default)
            {
                return BadRequest("Both 'desiredStartTime' and 'desiredEndTime' query parameters are required.");
            }

            if (desiredStartTime >= desiredEndTime)
            {
                return BadRequest("'desiredEndTime' must be after 'desiredStartTime'.");
            }

            var availableSlots = await _stationService.GetAvailableSlotsAsync(id, desiredStartTime, desiredEndTime);
            return Ok(availableSlots);
        }
    }
}
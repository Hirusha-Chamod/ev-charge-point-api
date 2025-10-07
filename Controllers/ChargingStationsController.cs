using ev_charge_point_api.Dtos;
using ev_charge_point_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ev_charge_point_api.Controllers
{
    [ApiController]
    [Route("api/stations")]
    public class ChargingStationsController : ControllerBase
    {
        private readonly IChargingStationService _stationService;

        public ChargingStationsController(IChargingStationService stationService)
        {
            _stationService = stationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stations = await _stationService.GetAllStationsAsync();
            return Ok(stations);
        }

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

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double longitude, [FromQuery] double latitude)
        {
            var stations = await _stationService.GetNearbyStationsAsync(longitude, latitude);
            return Ok(stations);
        }

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
    }
}
using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ev_charge_point_api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpGet("owner/{evOwnerNic}")]
        public async Task<IActionResult> GetByOwner(string evOwnerNic)
        {
            var bookings = await _bookingService.GetBookingsByOwnerAsync(evOwnerNic);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var booking = await _bookingService.CreateBookingAsync(createDto);
                if (booking == null) return BadRequest("Unable to create booking.");
                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBookingDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var ok = await _bookingService.UpdateBookingAsync(id, updateDto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Backoffice,StationOperator")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateBookingDto updateDto)
        {
            if (updateDto?.Status == null) return BadRequest(new { error = "Status is required." });
            try
            {
                var ok = await _bookingService.UpdateBookingStatusAsync(id, updateDto.Status.Value);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(string id, [FromBody] Dtos.CancelBookingDto? dto)
        {
            try
            {
                // If caller has privileged role, allow cancel directly
                var userIsPrivileged = User.IsInRole("Backoffice") || User.IsInRole("StationOperator");

                if (!userIsPrivileged)
                {
                    // Owner must supply their NIC in request body
                    if (dto == null || string.IsNullOrWhiteSpace(dto.EvOwnerNic))
                        return Forbid();

                    var booking = await _bookingService.GetBookingByIdAsync(id);
                    if (booking == null) return NotFound();
                    if (!string.Equals(booking.EvOwnerNic, dto.EvOwnerNic, StringComparison.OrdinalIgnoreCase))
                        return Forbid();
                }

                var ok = await _bookingService.CancelBookingAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

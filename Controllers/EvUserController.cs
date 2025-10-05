// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>EvUserController.cs</file>
// <author>Thilochana J M (IT22899224)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   API controller for managing EV users, including registration, login, profile updates, and deactivation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class EvUserController : ControllerBase
{
    private readonly EvUserService _service;
    private readonly AuthService _authService;

    public EvUserController(EvUserService service, AuthService authService)
    {
        _service = service;
        _authService = authService;
    }

    // Public login endpoint for EV owners (mobile app)
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginEvUserAsync(request.Email, request.Password);
        return Ok(result);
    }
    // Public registration endpoint used by the mobile app
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateEvUserDto dto)
    {
        var existing = await _service.GetByNicAsync(dto.Nic);
        if (existing != null)
            return Conflict("User with that NIC already exists");

        var user = new EvUser
        {
            Nic = dto.Nic,
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password
        };

        var created = await _service.RegisterAsync(user);
        return CreatedAtAction(nameof(Get), new { nic = created.Nic }, new { created.Nic, created.Name, created.Email });
    }

    [Authorize]
    [HttpGet("deactive")]
    public async Task<IActionResult> GetDeactiveUsers()
    {
        var users = await _service.GetAllDeactiveAsync();
        if (users == null) return NotFound();
        return Ok(users);
    }

    [Authorize]
    [HttpGet("{nic}")]
    public async Task<IActionResult> Get(string nic)
    {
        var user = await _service.GetByNicAsync(nic);
        if (user == null) return NotFound();
        return Ok(new { user.Nic, user.Name, user.Email, user.IsActive });
    }

    [Authorize]
    [HttpPut("{nic}")]
    public async Task<IActionResult> Update(string nic, [FromBody] UpdateEvUserDto dto)
    {
        var callerNic = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _service.GetByNicAsync(nic);
        if (existing == null) return NotFound();

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password)) existing.Password = dto.Password; // service will hash

        await _service.UpdateAsync(nic, existing, callerNic);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{nic}/deactivate")]
    public async Task<IActionResult> Deactivate(string nic, [FromBody] DeactivateEvUserDto dto)
    {
        var callerNic = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.Equals(nic, dto.Nic, StringComparison.OrdinalIgnoreCase))
            return BadRequest("NIC in URL and body must match");

        await _service.DeactivateAsync(nic, callerNic, dto.Reason);
        return NoContent();
    }

    // Reactivation only allowed for backoffice users
    [Authorize(Roles = "Backoffice")]
    [HttpPost("{nic}/reactivate")]
    public async Task<IActionResult> Reactivate(string nic)
    {
        var ok = await _service.ReactivateAsync(nic);
        if (!ok) return NotFound();
        return NoContent();
    }

}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

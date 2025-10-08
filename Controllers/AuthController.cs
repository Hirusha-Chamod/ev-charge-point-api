using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (token, refreshToken) = await _authService.AuthenticateAsync(request.Email, request.Password);
        return Ok(new
        {
            accessToken = token,
            refreshToken = refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var (newToken, newRefreshToken) = await _authService.RefreshAsync(refreshToken);
        return Ok(new { accessToken = newToken, refreshToken = newRefreshToken });
    }
}

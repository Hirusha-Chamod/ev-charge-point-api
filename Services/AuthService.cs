using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;
using ev_charge_point_api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserRepository _userRepository;
    private readonly EvUserRepository _evUserRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;

    public AuthService(IOptions<JwtSettings> jwtSettings, UserRepository userRepository, EvUserRepository evUserRepository, RefreshTokenRepository refreshTokenRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _evUserRepository = evUserRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<object> LoginAsync(string email, string password)
    {
        // Try EvUser first (mobile EV owners)
        var evUser = await _evUserRepository.GetByEmailAsync(email);
        if (evUser != null)
        {
            if (!evUser.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            if (!BCrypt.Net.BCrypt.Verify(password, evUser.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            var token = GenerateJwtTokenForEvUser(evUser);
            return new
            {
                token,
                user = new { Nic = evUser.Nic, evUser.Name, evUser.Email, Role = "EVOwner" }
            };
        }

        // Fallback to internal users
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        var jwtToken = GenerateJwtTokenForInternalUser(user);

        // Create refresh token for internal users
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.SaveTokenAsync(refreshToken);

        return new
        {
            token = jwtToken,
            refreshToken = refreshToken.Token,
            user = new
            {
                Id = user.Id,
                user.Name,
                user.Email,
                Role = user.Role.ToString()
            }
        };
    }

    public async Task<object> RefreshAsync(string refreshToken)
    {
        var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (existing == null || existing.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var user = await _userRepository.GetByIdAsync(existing.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        await _refreshTokenRepository.DeleteAsync(refreshToken); // Invalidate old refresh token

        var newJwtToken = GenerateJwtTokenForInternalUser(user);
        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        await _refreshTokenRepository.SaveTokenAsync(newRefreshToken);

        return new
        {
            token = newJwtToken,
            refreshToken = newRefreshToken.Token
        };
    }

    private string GenerateJwtTokenForEvUser(EvUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Nic),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "EVOwner")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateJwtTokenForInternalUser(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

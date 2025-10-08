using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;
using ev_charge_point_api.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly EvUserRepository _evUserRepository;

    public AuthService(UserRepository userRepository, RefreshTokenRepository refreshTokenRepository, JwtSettings jwtSettings)
    public AuthService(IOptions<JwtSettings> jwtSettings, UserRepository userRepository, EvUserRepository evUserRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings;
        _evUserRepository = evUserRepository;
    }

    public async Task<(string token, string refreshToken)> AuthenticateAsync(string email, string password)
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
            throw new Exception("Invalid credentials");

        var jwtToken = GenerateJwtToken(user);

        // 🔑 create refresh token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.SaveTokenAsync(refreshToken);

        return (jwtToken, refreshToken.Token);
    }

    public async Task<(string token, string refreshToken)> RefreshAsync(string token)
    {
        var existing = await _refreshTokenRepository.GetByTokenAsync(token);
        if (existing == null || existing.ExpiryDate < DateTime.UtcNow)
            throw new Exception("Invalid or expired refresh token");

        var user = await _userRepository.GetByIdAsync(existing.UserId);
        if (user == null)
            throw new Exception("User not found");

        await _refreshTokenRepository.DeleteAsync(token); // old one no longer valid

        var newToken = GenerateJwtToken(user);
        var newRefresh = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        await _refreshTokenRepository.SaveTokenAsync(newRefresh);

        return (newToken, newRefresh.Token);
    }

    private string GenerateJwtToken(User user)
        var token2 = GenerateJwtTokenForInternalUser(user);
        return new
        {
            token = token2,
            user = new
            {
                Id = user.Id,
                user.Name,
                user.Email,
                Role = user.Role.ToString()
            }
        };
    }

    private string GenerateJwtTokenForEvUser(EvUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

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
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateJwtTokenForInternalUser(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }
}

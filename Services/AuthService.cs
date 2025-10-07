using ev_charge_point_api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ev_charge_point_api.Settings;
using ev_charge_point_api.Repositories;

public class AuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserRepository _userRepository;

    public AuthService(IOptions<JwtSettings> jwtSettings, UserRepository userRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
    }

    public async Task<object> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        var token = GenerateJwtToken(user);

        return new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                Role = user.Role.ToString()
            }
        };
    }


    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

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
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

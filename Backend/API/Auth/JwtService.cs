using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HyteraAPI.Auth;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(string userId, string? email, string? role, string? bpCode, List<string>? scopes = null);
    (bool IsValid, string? UserId, string? Email, string? Role, string? BPCode, List<string>? Scopes) ValidateToken(string token);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GenerateTokenAsync(
        string userId,
        string? email,
        string? role,
        string? bpCode,
        List<string>? scopes = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(email))
            claims.Add(new Claim(ClaimTypes.Email, email));

        if (!string.IsNullOrEmpty(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        if (!string.IsNullOrEmpty(bpCode))
            claims.Add(new Claim("BPCode", bpCode));

        if (scopes?.Count > 0)
        {
            foreach (var scope in scopes)
            {
                claims.Add(new Claim("scope", scope));
            }
        }

        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "480");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public (bool IsValid, string? UserId, string? Email, string? Role, string? BPCode, List<string>? Scopes) ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, null, null, null, null, null);
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var bpCode = principal.FindFirst("BPCode")?.Value;
            var scopes = principal.FindAll("scope").Select(c => c.Value).ToList();

            return (true, userId, email, role, bpCode, scopes.Count > 0 ? scopes : null);
        }
        catch
        {
            return (false, null, null, null, null, null);
        }
    }
}

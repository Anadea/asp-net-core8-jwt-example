using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using asp_net_core_jwt_example.Enums;
using Microsoft.IdentityModel.Tokens;

namespace asp_net_core_jwt_example.Services;

public class TokenService(ILogger<TokenService> logger)
{
    private const int ExpirationMinutes = 60;
    
    private string? _validIssuer =
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
            .GetSection("JwtTokenSettings")["ValidIssuer"];

    private string? _validAudience =
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
            .GetSection("JwtTokenSettings")["ValidAudience"];

    private string? _secretKey =
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")[
            "SymmetricSecurityKey"];

    public string CreateToken(string phone, Guid cardGuid, Role role)
    {
        logger.LogTrace("IN CreateToken");
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(phone, cardGuid, role),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();

        logger.LogInformation("JWT Token created");

        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
        new(
            _validIssuer,
            _validAudience,
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private List<Claim> CreateClaims(string phone, Guid cardGuid, Role role)
    {
        var jwtSub =
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")[
                "JwtRegisteredClaimNamesSub"];

        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, jwtSub),
                new(JwtRegisteredClaimNames.Jti, cardGuid.ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new(ClaimTypes.Name, "name"), // use username/email here if needed
                new(ClaimTypes.Role, role.ToString())
            };

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            SecurityAlgorithms.HmacSha256);
    }

    // Validate a JWT token
    public ClaimsPrincipal ValidateJwtToken(string token)
    {
        // Encode the JWT secret key
        var byteArrayKey = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            // Create a token handler and validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _validIssuer,
                ValidAudience = _validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(byteArrayKey)
            }, out _);

            // Return the claims principal
            return claimsPrincipal;
        }
        catch (SecurityTokenExpiredException)
        {
            // Handle token expiration
            throw new ApplicationException("Token has expired.");
        }
    }
}
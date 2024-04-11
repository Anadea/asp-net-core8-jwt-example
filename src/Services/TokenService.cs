using JwtRoleAuthentication.Enums;
using JwtRoleAuthentication.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace JwtRoleAuthentication.Services;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private const int ExpirationMinutes = 60;
    private readonly ILogger<TokenService> _logger;

    public TokenService(ILogger<TokenService> logger)
    {
        _logger = logger;
    }

    public string CreateToken(string phone, Guid cardGuid, Role role)
    {
        _logger.LogTrace("IN CreateToken");
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(phone, cardGuid, role),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        
        _logger.LogInformation("JWT Token created");
        
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
        new(
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["ValidIssuer"],
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["ValidAudience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private List<Claim> CreateClaims(string phone, Guid cardGuid, Role role)
    {
        var jwtSub = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["JwtRegisteredClaimNamesSub"];
        
        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, jwtSub),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
                // todo not working
                // new Claim(ClaimTypes.Name, "name"), 
                // new Claim(ClaimTypes.Role, role.ToString())
                // new (ClaimTypes.NameIdentifier, cardGuid.ToString()),
                // new (ClaimTypes.UserData, phone)
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
        var symmetricSecurityKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["SymmetricSecurityKey"];
        
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
    
    // todo
    // // Validate a JWT token
    //    public ClaimsPrincipal ValidateJwtToken(string token)
    //    {
    //        // Retrieve the JWT secret from environment variables and encode it
    //        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);

    //        try
    //        {
    //            // Create a token handler and validate the token
    //            var tokenHandler = new JwtSecurityTokenHandler();
    //            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
    //                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
    //                IssuerSigningKey = new SymmetricSecurityKey(key)
    //            }, out SecurityToken validatedToken);

    //            // Return the claims principal
    //            return claimsPrincipal;
    //        }
    //        catch (SecurityTokenExpiredException)
    //        {
    //            // Handle token expiration
    //            throw new ApplicationException("Token has expired.");
    //        }
    //    }
}
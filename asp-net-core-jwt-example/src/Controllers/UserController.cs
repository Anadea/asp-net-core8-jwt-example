using asp_net_core_jwt_example.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace asp_net_core_jwt_example.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController(TokenService tokenService, ILogger<TokenService> logger) : ControllerBase
{
    /*
     *  This request is private, only for authorized users.
     */
    [HttpGet(Name = "UserDetails")]
    [Authorize]
    public ActionResult<string> GetUser()
    {
        // This is a hardcoded example. 
        Dictionary<Guid, string> userDictionary = new Dictionary<Guid, string>
        {
            { Guid.Parse("41f0cca7-2537-404e-b408-ae93787835bd"), "John" },
            { Guid.NewGuid(), "Jane" },
            { Guid.NewGuid(), "Alice" }
        };

        try
        {
            // Extract the JWT (Json Web Token) from the Authorization header of the HTTP request.
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Validate the JWT and retrieve claims about the user.
            var claimsPrincipal = tokenService.ValidateJwtToken(token);

            // Check if the user is authenticated. If not, return an unauthorized response.
            if (claimsPrincipal.Identity?.IsAuthenticated != true)
            {
                return Unauthorized("Token has expired.");
            }

            // Extract the user's card guid from the JWT claims.
            Guid cardGuid = Guid.Parse(claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value!);
            logger.LogInformation(cardGuid.ToString());

            // Check if the card guid exists in the dictionary.
            if (!userDictionary.TryGetValue(cardGuid, out var userName)) return NotFound("USER not found");
            return $"Hello {userName}";
        }
        catch (SecurityTokenExpiredException e)
        {
            Console.WriteLine(e);
            return Unauthorized("Token has expired.");
        }
    }
}
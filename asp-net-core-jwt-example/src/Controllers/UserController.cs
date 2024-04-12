using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_core_jwt_example.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController(ILogger<UserController> logger) : ControllerBase
{
    // This is a hardcoded example. 
    private static readonly Dictionary<Guid, string> UserDictionary = new()
    {
        { Guid.Parse("41f0cca7-2537-404e-b408-ae93787835bd"), "John" },
        { Guid.NewGuid(), "Jane" },
        { Guid.NewGuid(), "Alice" }
    };

    /*
     *  This request is private, only for authorized users.
     *  Authorization header should contain Bearer token.
     */
    [HttpGet(Name = "UserDetails")]
    [Authorize]
    public ActionResult<string> GetUser()
    {
        // Extract the user's card guid from context.
        Guid cardGuid = (Guid)HttpContext.Items["CardGuid"]!;
        logger.LogInformation(cardGuid.ToString());

        if (!UserDictionary.TryGetValue(cardGuid, out var userName)) return NotFound("USER not found");
        return Ok($"Hello {userName}");
    }
}
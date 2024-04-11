using asp_net_core_jwt_example.Enums;
using asp_net_core_jwt_example.Models;
using asp_net_core_jwt_example.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_core_jwt_example.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthenticationController(TokenService tokenService) : ControllerBase
{

    /*
     * This request is public.
     * Inside should be a logic to create OTP, save the pair, phone and OTP, into DB
     * and send OTP on the client phone.
     */
    [HttpPost]
    public ActionResult<string> Auth([FromBody] AuthRequest request)
    {
        var responseMessage = $"OTP was sent to {request.Phone} successfully.";

        return Ok(responseMessage);
    }

    /*
     * This request is public.
     * This method handles a public request to verify a pair, phone, and OTP against the database.
     * If the pair isn't successfully verified, a 401 error is returned to the client.
     * If the pair is successfully verified, a JWT token is generated for further authentication.
     * Jwt token should have a claim 'card_guid', so mobile client could parse it and use.
     * exampleCardGuid - This one is hardcoded. Should be taken from DB or generated for new user.
     * exampleUserRole - You can add to claims roles, this one is hardcoded. Take the real user role from DB if needed.
     */
    [HttpPost("verify")]
    public ActionResult<string> VerifyOtp([FromBody] VerifyOtpRequest otpRequest)
    {
        if (otpRequest.Otp != 123456 && otpRequest.Phone != "+123456")
        {
            return Unauthorized();
        }
        var phone = otpRequest.Phone ?? throw new InvalidOperationException("Phone is null.");
        var exampleCardGuid = Guid.Parse("41f0cca7-2537-404e-b408-ae93787835bd");
        const Role exampleUserRole = Role.User;
        var accessToken = tokenService.CreateToken(phone, exampleCardGuid, exampleUserRole); 
        return Ok(accessToken);
    }
}

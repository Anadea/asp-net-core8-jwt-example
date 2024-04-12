using asp_net_core_jwt_example.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace asp_net_core_jwt_example.Middlewares;

public class JwtMiddleware(TokenService tokenService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Get the token from the Authorization header
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!String.IsNullOrEmpty(token))
        {
            try
            {
                // Verify the token using the JwtSecurityTokenHandlerWrapper
                var claimsPrincipal = tokenService.ValidateJwtToken(token);

                // Extract the user CardGuid from the token
                var cardGuidStr = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;
                
                // Store the user card ID in the HttpContext items for later use
                context.Items["CardGuid"] = Guid.Parse(cardGuidStr);

                // You can also add to context other key which you have in JWT token.
            }
            catch (Exception)
            {
                // If the token is invalid, throw an exception
                context.Response.StatusCode = new UnauthorizedResult().StatusCode;
            }
        }

        // Continue processing the request
        await next(context);
    }
}
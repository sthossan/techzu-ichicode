using Microsoft.AspNetCore.Http;
using Server.Shared.Models;
using System.Security.Claims;
using Shared.Enums;

namespace Server.Api.Middleware
{
    public class RbacMiddleware
    {
        private readonly RequestDelegate _next;

        public RbacMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip validation for public endpoints (e.g., login, swagger)
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.Contains("/auth/") || path.Contains("/swagger/")))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                
                // If the user is authenticated but has no valid system role, block them
                var validRoles = Enum.GetNames(typeof(UserRole));
                if (!roles.Any(r => validRoles.Contains(r)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    
                    var response = ApiResponse.Error(
                        message: "Access Denied: Your account does not have a valid role assigned.",
                        statusCode: StatusCodes.Status403Forbidden
                    );
                    
                    await context.Response.WriteAsJsonAsync(response);
                    return;
                }
            }

            await _next(context);
        }
    }
}

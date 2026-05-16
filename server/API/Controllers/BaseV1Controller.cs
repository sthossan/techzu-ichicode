using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Server.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class BaseV1Controller : ControllerBase
    {
        protected string UserId
        {
            get
            {
                var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!string.IsNullOrEmpty(userClaim)) return userClaim;

                return string.Empty;
            }
        }

        protected IEnumerable<string> Roles
        {
            get
            {
                //return User?.FindAll(ClaimTypes.Role).Select(r => r.Value) ?? Enumerable.Empty<string>();

                if (User == null) return Enumerable.Empty<string>();

                var roleClaims = User.FindAll(ClaimTypes.Role)
                                .Union(User.FindAll("roles"))
                                .Union(User.FindAll("role"));

                return roleClaims.Select(r => r.Value).Distinct();
            }

        }
    }
}

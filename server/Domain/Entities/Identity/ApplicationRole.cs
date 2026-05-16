using Microsoft.AspNetCore.Identity;

namespace Server.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}

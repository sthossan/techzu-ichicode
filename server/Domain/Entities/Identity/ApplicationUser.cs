using Microsoft.AspNetCore.Identity;
using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        // Additional properties from existing User entity
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}

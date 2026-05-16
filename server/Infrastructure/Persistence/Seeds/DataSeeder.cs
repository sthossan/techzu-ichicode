using Microsoft.AspNetCore.Identity;
using Server.Domain.Entities.Identity;
using Server.Infrastructure.Persistence.Data;
using Shared.Enums;
using System.Security.Claims;

namespace Server.Infrastructure.Persistence.Seeds
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndUsersAsync(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // 1. Seed Roles
            var roles = Enum.GetNames(typeof(UserRole));
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }

            // 2. Seed Users
            await CreateUserAsync(userManager, "requestor@techzu.com", "Requestor123!", UserRole.Requestor);
            await CreateUserAsync(userManager, "manager@techzu.com", "Manager123!", UserRole.Manager);
            await CreateUserAsync(userManager, "finance@techzu.com", "Finance123!", UserRole.FinanceAdmin);
            await CreateUserAsync(userManager, "admin@techzu.com", "Admin123!", UserRole.SystemAdmin);
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, string email, string password, UserRole role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = role.ToString(),
                    LastName = "User",
                    DisplayName = role.ToString() + " User"
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role.ToString());
                    // Add role claim as well for easier middleware check if needed
                    await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role.ToString()));
                }
            }
        }
    }
}

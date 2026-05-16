using Server.Domain.Entities.Identity;

namespace Server.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllUserRefreshTokensAsync(string userId);
        Task CleanupExpiredTokensAsync();
    }
}

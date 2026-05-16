using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Interfaces;
using Server.Application.Interfaces.Core;
using Server.Domain.Entities.Identity;
using Server.Domain.Entities.Authorization;

namespace Server.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Generate a cryptographically secure random token
            var guidBytes = Guid.NewGuid().ToByteArray();
            var timeBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
            var allBytes = new byte[guidBytes.Length + timeBytes.Length];
            Buffer.BlockCopy(guidBytes, 0, allBytes, 0, guidBytes.Length);
            Buffer.BlockCopy(timeBytes, 0, allBytes, guidBytes.Length, timeBytes.Length);
            
            var refreshTokenString = Convert.ToBase64String(allBytes);
            
            var tokenEntity = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days expiry
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(tokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return refreshTokenString;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.Repository<RefreshToken>()
                .GetQueryable()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                return false;

            // Check if user is still active
            if (token.User == null || !token.User.IsActive)
                return false;

            return true;
        }

        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.Repository<RefreshToken>()
                .GetQueryable()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                return null;

            return token.User;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.Repository<RefreshToken>()
                .GetQueryable()
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token != null)
            {
                token.IsRevoked = true;
                _unitOfWork.Repository<RefreshToken>().Update(token);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserRefreshTokensAsync(string userId)
        {
            try
            {
                var userTokens = await _unitOfWork.Repository<RefreshToken>()
                    .GetQueryable()
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                if (userTokens.Any())
                {
                    foreach (var token in userTokens)
                    {
                        token.IsRevoked = true;
                        _unitOfWork.Repository<RefreshToken>().Update(token);
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking refresh tokens for user {userId}: {ex.Message}");
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _unitOfWork.Repository<RefreshToken>()
                .GetQueryable()
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow.AddDays(-1)) // Keep expired tokens for 1 day
                .ToListAsync();

            if (expiredTokens.Any())
            {
                foreach (var token in expiredTokens)
                {
                    await _unitOfWork.Repository<RefreshToken>().DeleteAsync(token);
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

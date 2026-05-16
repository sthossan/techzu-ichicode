using Server.Application.DTOs.Auth;
using Server.Shared.Models;

namespace Server.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> LoginAsync(LoginRequest request);
        Task<ApiResponse> RefreshTokenAsync(string refreshToken);
    }
}

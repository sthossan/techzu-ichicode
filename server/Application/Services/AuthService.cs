using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.DTOs.Auth;
using Server.Application.Interfaces;
using Server.Domain.Entities.Identity;
using Server.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtOptions,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ApiResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Username) 
                       ?? await _userManager.FindByNameAsync(request.Username);

            if (user == null || !user.IsActive)
                return ApiResponse.Error(message: "Invalid credentials or account inactive.", statusCode: 401);

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return ApiResponse.Error(message: "Invalid credentials.", statusCode: 401);

            return await GenerateAuthResponse(user);
        }

        public async Task<ApiResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = await _refreshTokenService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null || !user.IsActive)
                return ApiResponse.Error(message: "Invalid or expired refresh token.", statusCode: 401);

            // Revoke old token
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

            return await GenerateAuthResponse(user);
        }

        private async Task<ApiResponse> GenerateAuthResponse(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("DisplayName", user.DisplayName ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.DurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            var responseData = new LoginResponseData
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                expires_in = (int)(expires - DateTime.UtcNow).TotalSeconds,
                id_token = accessToken // Simplified for this context
            };

            return ApiResponse.Single(responseData, message: "Login successful.");
        }
    }
}

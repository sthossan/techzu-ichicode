using Server.Shared.Models;

namespace Server.Application.DTOs.Auth
{
    public class LoginDto { }

    public record LoginRequest(string Username, string Password);
    public record AuthResponse(string AccessToken, string? RefreshToken = null);

    // New DTOs for the specified login response format
    public class LoginResponseData
    {
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string token_type { get; set; } = "Bearer";
        public string id_token { get; set; } = string.Empty;
        public string scope { get; set; } = string.Empty;
    }

    public class ExternalProviderStatusDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<string> ConnectedProviders { get; set; } = new();
        public bool HasAnyProvider => ConnectedProviders.Any();
    }
}

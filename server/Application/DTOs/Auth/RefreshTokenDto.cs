namespace Server.Application.DTOs.Auth
{
    public record RefreshTokenRequest(string RefreshToken);
    
    public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);
}

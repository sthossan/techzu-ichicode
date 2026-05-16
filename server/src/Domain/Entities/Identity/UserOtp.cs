using System.ComponentModel.DataAnnotations;

namespace Server.Domain.Entities.Identity
{
    public class UserOtp
    {
        public Guid Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string OtpCodeHash { get; set; } = string.Empty;
        
        [Required]
        public string OtpType { get; set; } = string.Empty; // "login", "password_reset", "email_verification"
        
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int AttemptCount { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        
        public ApplicationUser User { get; set; } = null!;
    }
}

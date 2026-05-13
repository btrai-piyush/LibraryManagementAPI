using System.ComponentModel.DataAnnotations;

namespace LibraryManagementClassLib.Entities
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string TokenHash { get; set; }

        [Required]
        public DateTime ExpiresAtUtc { get; set; }

        [Required]
        public DateTime CreatedAtUtc { get; set; }

        public string? CreatedByIp { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsRevoked => RevokedAtUtc != null;
        public bool IsActive => !IsExpired && !IsRevoked;

        public User User { get; set; }
    }
}

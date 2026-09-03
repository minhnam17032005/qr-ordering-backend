using QROrdering.Domain.Entities;
using QROrdering.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

public class UserSession : BaseEntity
{

    public Guid UserId { get; set; }

    public string RefreshTokenHash { get; set; } = null!;

    public string? DeviceName { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime ExpiredAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime LastAccessAt { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
}
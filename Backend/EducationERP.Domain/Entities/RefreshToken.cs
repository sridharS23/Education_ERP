using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Refresh token entity for JWT token refresh mechanism
/// </summary>
public class RefreshToken : BaseEntity
{
    private RefreshToken() { } // For EF Core

    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public string? CreatedByIp { get; private set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string? createdByIp = null)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedByIp = createdByIp,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke(string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}

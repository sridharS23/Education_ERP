using EducationERP.Domain.Common;
using EducationERP.Domain.Exceptions;

namespace EducationERP.Domain.Entities;

/// <summary>
/// User entity representing system users (Admin, Faculty, Student, Parent)
/// </summary>
public class User : BaseEntity
{
    private User() { } // For EF Core

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsEmailVerified { get; private set; } = false;
    public string? EmailVerificationToken { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetExpires { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual Student? Student { get; private set; }
    public virtual Faculty? Faculty { get; private set; }
    public virtual Parent? Parent { get; private set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    /// <summary>
    /// Factory method to create a new user
    /// </summary>
    public static User Create(string email, string firstName, string lastName, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required");

        if (!IsValidEmail(email))
            throw new DomainException("Invalid email format");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant().Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            IsActive = true,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty");

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        EmailVerificationToken = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmailVerificationToken(string token)
    {
        EmailVerificationToken = token;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordResetToken(string token, DateTime expiresAt)
    {
        PasswordResetToken = token;
        PasswordResetExpires = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetExpires = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}

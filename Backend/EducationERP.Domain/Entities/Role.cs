using EducationERP.Domain.Common;
using EducationERP.Domain.Exceptions;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Role entity for role-based access control
/// </summary>
public class Role : BaseEntity
{
    private Role() { } // For EF Core

    public string RoleName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; } = false;

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Role Create(string roleName, string? description = null, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new DomainException("Role name is required");

        return new Role
        {
            Id = Guid.NewGuid(),
            RoleName = roleName.Trim(),
            Description = description?.Trim(),
            IsSystemRole = isSystemRole,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string roleName, string? description)
    {
        if (IsSystemRole)
            throw new DomainException("Cannot modify system roles");

        if (string.IsNullOrWhiteSpace(roleName))
            throw new DomainException("Role name is required");

        RoleName = roleName.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}

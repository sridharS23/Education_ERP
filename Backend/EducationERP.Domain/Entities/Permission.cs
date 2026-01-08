using EducationERP.Domain.Common;
using EducationERP.Domain.Exceptions;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Permission entity for granular access control
/// </summary>
public class Permission : BaseEntity
{
    private Permission() { } // For EF Core

    public string PermissionName { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Permission Create(string permissionName, string resource, string action, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
            throw new DomainException("Permission name is required");

        if (string.IsNullOrWhiteSpace(resource))
            throw new DomainException("Resource is required");

        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Action is required");

        return new Permission
        {
            Id = Guid.NewGuid(),
            PermissionName = permissionName.ToLowerInvariant().Trim(),
            Resource = resource.ToLowerInvariant().Trim(),
            Action = action.ToLowerInvariant().Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}

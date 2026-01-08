using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Many-to-many relationship between Roles and Permissions
/// </summary>
public class RolePermission : BaseEntity
{
    private RolePermission() { } // For EF Core

    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime GrantedAt { get; private set; }

    // Navigation properties
    public virtual Role Role { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;

    public static RolePermission Create(Guid roleId, Guid permissionId)
    {
        return new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permissionId,
            GrantedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }
}

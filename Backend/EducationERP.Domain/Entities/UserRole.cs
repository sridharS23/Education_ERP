using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Many-to-many relationship between Users and Roles
/// </summary>
public class UserRole : BaseEntity
{
    private UserRole() { } // For EF Core

    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;

    public static UserRole Create(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        return new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}

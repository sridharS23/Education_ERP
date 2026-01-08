namespace EducationERP.Domain.Common;

/// <summary>
/// Base entity class for all domain entities
/// Provides common properties like Id, timestamps, and audit fields
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User ID who created this entity
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// User ID who last updated this entity
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}

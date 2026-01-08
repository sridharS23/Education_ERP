using EducationERP.Domain.Common;
using EducationERP.Domain.Exceptions;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Parent entity representing student guardians
/// </summary>
public class Parent : BaseEntity
{
    private Parent() { } // For EF Core

    public Guid UserId { get; private set; }
    public string Relationship { get; private set; } = string.Empty;
    public string? Occupation { get; private set; }
    public decimal? AnnualIncome { get; private set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;

    public static Parent Create(Guid userId, string relationship)
    {
        if (string.IsNullOrWhiteSpace(relationship))
            throw new DomainException("Relationship is required");

        return new Parent
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Relationship = relationship.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string relationship, string? occupation, decimal? annualIncome)
    {
        if (string.IsNullOrWhiteSpace(relationship))
            throw new DomainException("Relationship is required");

        if (annualIncome.HasValue && annualIncome.Value < 0)
            throw new DomainException("Annual income cannot be negative");

        Relationship = relationship.Trim();
        Occupation = occupation?.Trim();
        AnnualIncome = annualIncome;
        UpdatedAt = DateTime.UtcNow;
    }
}

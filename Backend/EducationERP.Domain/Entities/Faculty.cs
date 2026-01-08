using EducationERP.Domain.Common;
using EducationERP.Domain.Exceptions;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Faculty entity representing teaching staff
/// </summary>
public class Faculty : BaseEntity
{
    private Faculty() { } // For EF Core

    public Guid UserId { get; private set; }
    public string EmployeeId { get; private set; } = string.Empty;
    public string? Department { get; private set; }
    public string? Designation { get; private set; }
    public string? Qualification { get; private set; }
    public string? Specialization { get; private set; }
    public DateTime DateOfJoining { get; private set; }
    public string? EmploymentType { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual User User { get; private set; } = null!;

    public static Faculty Create(
        Guid userId,
        string employeeId,
        DateTime dateOfJoining,
        string? department = null,
        string? designation = null)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            throw new DomainException("Employee ID is required");

        if (dateOfJoining > DateTime.Today)
            throw new DomainException("Date of joining cannot be in the future");

        return new Faculty
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EmployeeId = employeeId.Trim(),
            DateOfJoining = dateOfJoining.Date,
            Department = department?.Trim(),
            Designation = designation?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfessionalDetails(
        string? department,
        string? designation,
        string? qualification,
        string? specialization,
        string? employmentType)
    {
        Department = department?.Trim();
        Designation = designation?.Trim();
        Qualification = qualification?.Trim();
        Specialization = specialization?.Trim();
        EmploymentType = employmentType?.Trim();
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

    public int GetYearsOfService()
    {
        var years = DateTime.Today.Year - DateOfJoining.Year;
        if (DateOfJoining.Date > DateTime.Today.AddYears(-years)) years--;
        return years;
    }
}

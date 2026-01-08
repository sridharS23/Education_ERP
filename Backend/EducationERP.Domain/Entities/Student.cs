using EducationERP.Domain.Common;
using EducationERP.Domain.Enums;
using EducationERP.Domain.Exceptions;

namespace EducationERP.Domain.Entities;

/// <summary>
/// Student entity representing enrolled students
/// </summary>
public class Student : BaseEntity
{
    private Student() { } // For EF Core

    public Guid UserId { get; private set; }
    public string AdmissionNumber { get; private set; } = string.Empty;
    public DateTime AdmissionDate { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? BloodGroup { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string Country { get; private set; } = "India";
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public EnrollmentStatus EnrollmentStatus { get; private set; }
    public string AcademicYear { get; private set; } = string.Empty;

    // Navigation properties
    public virtual User User { get; private set; } = null!;

    public static Student Create(
        Guid userId,
        string admissionNumber,
        DateTime dateOfBirth,
        Gender gender,
        string academicYear)
    {
        if (string.IsNullOrWhiteSpace(admissionNumber))
            throw new DomainException("Admission number is required");

        if (dateOfBirth >= DateTime.Today)
            throw new DomainException("Date of birth must be in the past");

        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

        if (age < 5 || age > 100)
            throw new DomainException("Invalid age for admission");

        if (string.IsNullOrWhiteSpace(academicYear))
            throw new DomainException("Academic year is required");

        return new Student
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AdmissionNumber = admissionNumber.Trim(),
            AdmissionDate = DateTime.UtcNow,
            DateOfBirth = dateOfBirth.Date,
            Gender = gender,
            EnrollmentStatus = EnrollmentStatus.Pending,
            AcademicYear = academicYear.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdatePersonalDetails(
        DateTime dateOfBirth,
        Gender gender,
        string? bloodGroup,
        string? address,
        string? city,
        string? state,
        string? postalCode,
        string country)
    {
        DateOfBirth = dateOfBirth.Date;
        Gender = gender;
        BloodGroup = bloodGroup?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        PostalCode = postalCode?.Trim();
        Country = string.IsNullOrWhiteSpace(country) ? "India" : country.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmergencyContact(string contactName, string contactPhone)
    {
        if (string.IsNullOrWhiteSpace(contactName))
            throw new DomainException("Emergency contact name is required");

        if (string.IsNullOrWhiteSpace(contactPhone))
            throw new DomainException("Emergency contact phone is required");

        EmergencyContactName = contactName.Trim();
        EmergencyContactPhone = contactPhone.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        EnrollmentStatus = EnrollmentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        EnrollmentStatus = EnrollmentStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        EnrollmentStatus = EnrollmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw()
    {
        EnrollmentStatus = EnrollmentStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetAge()
    {
        var age = DateTime.Today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
        return age;
    }
}

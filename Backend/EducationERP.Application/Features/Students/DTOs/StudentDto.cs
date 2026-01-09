using EducationERP.Domain.Enums;

namespace EducationERP.Application.Features.Students.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "India";
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    
    // User details
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateStudentDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "India";
}

public class UpdateStudentDto
{
    public Guid Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "India";
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

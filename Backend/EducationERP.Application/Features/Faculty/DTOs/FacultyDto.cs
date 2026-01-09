using EducationERP.Domain.Enums;

namespace EducationERP.Application.Features.Faculty.DTOs;

public class FacultyDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public DateTime DateOfJoining { get; set; }
    public string? EmploymentType { get; set; }
    public bool IsActive { get; set; }
    
    // User details
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateFacultyDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime DateOfJoining { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
}

public class UpdateFacultyDto
{
    public Guid Id { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public string? EmploymentType { get; set; }
}

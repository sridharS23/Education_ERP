using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Domain.Entities;
using EducationERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime DateOfBirth,
    Gender Gender,
    string RoleType = "Student" // Default to Student role
) : IRequest<Result<RegisterResponse>>;

public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            return Result<RegisterResponse>.Failure("User with this email already exists");
        }

        // Create user using factory method
        var user = User.Create(
            request.Email,
            request.FirstName,
            request.LastName,
            request.PhoneNumber
        );

        // Set password
        user.SetPassword(_passwordHasher.HashPassword(request.Password));

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign role
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == request.RoleType, cancellationToken);

        if (role != null)
        {
            var userRole = UserRole.Create(user.Id, role.Id);
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Create Student or Faculty record based on role
        if (request.RoleType == "Student")
        {
            var student = Student.Create(
                user.Id,
                $"ADM{DateTime.UtcNow:yyyyMMddHHmmss}", // Generate admission number
                request.DateOfBirth,
                request.Gender,
                DateTime.UtcNow.Year.ToString() // Academic year
            );
            _context.Students.Add(student);
        }
        else if (request.RoleType == "Faculty")
        {
            var faculty = Domain.Entities.Faculty.Create(
                user.Id,
                $"EMP{DateTime.UtcNow:yyyyMMddHHmmss}", // Generate employee ID
                DateTime.UtcNow // Date of joining
            );
            _context.Faculties.Add(faculty);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var response = new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Message = "User registered successfully"
        };

        return Result<RegisterResponse>.Success(response);
    }
}

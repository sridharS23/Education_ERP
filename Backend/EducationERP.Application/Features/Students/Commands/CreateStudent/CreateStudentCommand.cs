using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Students.DTOs;
using EducationERP.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Students.Commands.CreateStudent;

public record CreateStudentCommand(CreateStudentDto Student) : IRequest<Result<StudentDto>>;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public CreateStudentCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        // Create user using factory method
        var user = User.Create(
            request.Student.Email,
            request.Student.FirstName,
            request.Student.LastName,
            request.Student.PhoneNumber
        );

        // Set password
        user.SetPassword(_passwordHasher.HashPassword(request.Student.Password));

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign Student role
        var studentRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "Student", cancellationToken);

        if (studentRole != null)
        {
            var userRole = UserRole.Create(user.Id, studentRole.Id);
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Create student record using factory method
        var student = Student.Create(
            user.Id,
            request.Student.AdmissionNumber,
            request.Student.DateOfBirth,
            request.Student.Gender,
            request.Student.AcademicYear
        );

        // Update personal details if provided
        if (!string.IsNullOrWhiteSpace(request.Student.Address) ||
            !string.IsNullOrWhiteSpace(request.Student.City))
        {
            student.UpdatePersonalDetails(
                request.Student.DateOfBirth,
                request.Student.Gender,
                request.Student.BloodGroup,
                request.Student.Address,
                request.Student.City,
                request.Student.State,
                request.Student.PostalCode,
                request.Student.Country
            );
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        // Load the student with User navigation property for mapping
        var createdStudent = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == student.Id, cancellationToken);

        var studentDto = _mapper.Map<StudentDto>(createdStudent);
        return Result<StudentDto>.Success(studentDto);
    }
}

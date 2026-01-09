using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Faculty.DTOs;
using EducationERP.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Faculty.Commands.CreateFaculty;

public record CreateFacultyCommand(CreateFacultyDto Faculty) : IRequest<Result<FacultyDto>>;

public class CreateFacultyCommandHandler : IRequestHandler<CreateFacultyCommand, Result<FacultyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public CreateFacultyCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<Result<FacultyDto>> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
    {
        // Create user using factory method
        var user = User.Create(
            request.Faculty.Email,
            request.Faculty.FirstName,
            request.Faculty.LastName,
            request.Faculty.PhoneNumber
        );

        // Set password
        user.SetPassword(_passwordHasher.HashPassword(request.Faculty.Password));

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign Faculty role
        var facultyRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "Faculty", cancellationToken);

        if (facultyRole != null)
        {
            var userRole = UserRole.Create(user.Id, facultyRole.Id);
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Create faculty record using factory method
        var faculty = Domain.Entities.Faculty.Create(
            user.Id,
            request.Faculty.EmployeeId,
            request.Faculty.DateOfJoining,
            request.Faculty.Department,
            request.Faculty.Designation
        );

        // Update professional details if provided
        if (!string.IsNullOrWhiteSpace(request.Faculty.Qualification) ||
            !string.IsNullOrWhiteSpace(request.Faculty.Specialization))
        {
            faculty.UpdateProfessionalDetails(
                request.Faculty.Department,
                request.Faculty.Designation,
                request.Faculty.Qualification,
                request.Faculty.Specialization,
                null // Employment type
            );
        }

        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync(cancellationToken);

        // Load the faculty with User navigation property for mapping
        var createdFaculty = await _context.Faculties
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == faculty.Id, cancellationToken);

        var facultyDto = _mapper.Map<FacultyDto>(createdFaculty);
        return Result<FacultyDto>.Success(facultyDto);
    }
}

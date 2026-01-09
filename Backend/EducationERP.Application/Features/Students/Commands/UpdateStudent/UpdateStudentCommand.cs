using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Students.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Students.Commands.UpdateStudent;

public record UpdateStudentCommand(UpdateStudentDto Student) : IRequest<Result<StudentDto>>;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateStudentCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == request.Student.Id, cancellationToken);

        if (student == null)
        {
            return Result<StudentDto>.Failure("Student not found");
        }

        // Update personal details using domain method
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

        // Update emergency contact if provided
        if (!string.IsNullOrWhiteSpace(request.Student.EmergencyContactName) &&
            !string.IsNullOrWhiteSpace(request.Student.EmergencyContactPhone))
        {
            student.SetEmergencyContact(
                request.Student.EmergencyContactName,
                request.Student.EmergencyContactPhone
            );
        }

        await _context.SaveChangesAsync(cancellationToken);

        var studentDto = _mapper.Map<StudentDto>(student);
        return Result<StudentDto>.Success(studentDto);
    }
}

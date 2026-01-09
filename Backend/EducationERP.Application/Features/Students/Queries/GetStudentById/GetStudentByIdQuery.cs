using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Students.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Students.Queries.GetStudentById;

public record GetStudentByIdQuery(Guid Id) : IRequest<Result<StudentDto>>;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStudentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student == null)
        {
            return Result<StudentDto>.Failure("Student not found");
        }

        var studentDto = _mapper.Map<StudentDto>(student);
        return Result<StudentDto>.Success(studentDto);
    }
}

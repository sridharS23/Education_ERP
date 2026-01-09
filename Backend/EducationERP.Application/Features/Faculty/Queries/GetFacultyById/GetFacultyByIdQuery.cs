using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Faculty.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Faculty.Queries.GetFacultyById;

public record GetFacultyByIdQuery(Guid Id) : IRequest<Result<FacultyDto>>;

public class GetFacultyByIdQueryHandler : IRequestHandler<GetFacultyByIdQuery, Result<FacultyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFacultyByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<FacultyDto>> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
    {
        var faculty = await _context.Faculties
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (faculty == null)
        {
            return Result<FacultyDto>.Failure("Faculty not found");
        }

        var facultyDto = _mapper.Map<FacultyDto>(faculty);
        return Result<FacultyDto>.Success(facultyDto);
    }
}

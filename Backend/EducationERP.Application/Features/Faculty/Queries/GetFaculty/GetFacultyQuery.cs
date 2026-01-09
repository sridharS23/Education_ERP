using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Faculty.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Faculty.Queries.GetFaculty;

public record GetFacultyQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null) 
    : IRequest<Result<PaginatedList<FacultyDto>>>;

public class GetFacultyQueryHandler : IRequestHandler<GetFacultyQuery, Result<PaginatedList<FacultyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFacultyQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<FacultyDto>>> Handle(GetFacultyQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Faculties
            .Include(f => f.User)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(f =>
                f.User.FirstName.ToLower().Contains(searchTerm) ||
                f.User.LastName.ToLower().Contains(searchTerm) ||
                f.User.Email.ToLower().Contains(searchTerm) ||
                f.EmployeeId.ToLower().Contains(searchTerm));
        }

        // Order by creation date
        query = query.OrderByDescending(f => f.CreatedAt);

        // Get paginated results
        var faculty = await PaginatedList<Domain.Entities.Faculty>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize);

        var facultyDtos = new PaginatedList<FacultyDto>(
            _mapper.Map<List<FacultyDto>>(faculty.Items),
            faculty.TotalCount,
            faculty.PageNumber,
            faculty.PageSize);

        return Result<PaginatedList<FacultyDto>>.Success(facultyDtos);
    }
}

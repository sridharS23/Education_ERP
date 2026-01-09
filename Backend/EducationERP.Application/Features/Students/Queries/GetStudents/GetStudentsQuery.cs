using AutoMapper;
using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Students.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Students.Queries.GetStudents;

public record GetStudentsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null) 
    : IRequest<Result<PaginatedList<StudentDto>>>;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Result<PaginatedList<StudentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStudentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Students
            .Include(s => s.User)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.User.FirstName.ToLower().Contains(searchTerm) ||
                s.User.LastName.ToLower().Contains(searchTerm) ||
                s.User.Email.ToLower().Contains(searchTerm) ||
                s.AdmissionNumber.ToLower().Contains(searchTerm));
        }

        // Order by creation date
        query = query.OrderByDescending(s => s.CreatedAt);

        // Get paginated results
        var students = await PaginatedList<Domain.Entities.Student>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize);

        var studentDtos = new PaginatedList<StudentDto>(
            _mapper.Map<List<StudentDto>>(students.Items),
            students.TotalCount,
            students.PageNumber,
            students.PageSize);

        return Result<PaginatedList<StudentDto>>.Success(studentDtos);
    }
}

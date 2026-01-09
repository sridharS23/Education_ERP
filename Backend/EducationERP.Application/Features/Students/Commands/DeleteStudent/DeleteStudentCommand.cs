using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Students.Commands.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteStudentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student == null)
        {
            return Result.Failure("Student not found");
        }

        // Use domain method to withdraw student (soft delete)
        student.Withdraw();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

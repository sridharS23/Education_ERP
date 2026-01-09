using EducationERP.Application.Common.Interfaces;
using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Auth.Commands.Login;
using EducationERP.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find refresh token
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked, cancellationToken);

        if (refreshToken == null)
        {
            return Result<LoginResponse>.Failure("Invalid refresh token");
        }

        // Check if token is expired
        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result<LoginResponse>.Failure("Refresh token has expired");
        }

        var user = refreshToken.User;

        // Get user roles
        var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Revoke old refresh token using domain method
        refreshToken.Revoke(newRefreshToken);

        // Save new refresh token using factory method
        var newRefreshTokenEntity = Domain.Entities.RefreshToken.Create(
            user.Id,
            newRefreshToken,
            DateTime.UtcNow.AddDays(7)
        );

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            }
        };

        return Result<LoginResponse>.Success(response);
    }
}

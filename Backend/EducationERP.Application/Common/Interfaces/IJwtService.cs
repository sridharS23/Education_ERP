using EducationERP.Domain.Entities;

namespace EducationERP.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}

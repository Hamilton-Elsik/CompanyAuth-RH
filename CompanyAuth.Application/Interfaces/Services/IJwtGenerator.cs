using CompanyAuth.Domain.Entities;

namespace CompanyAuth.Application.Interfaces.Services;

public interface IJwtGenerator
{
    string GenerateToken(User user, Role role);
}
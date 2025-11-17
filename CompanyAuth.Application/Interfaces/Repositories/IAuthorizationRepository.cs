using CompanyAuth.Domain.Entities;

namespace CompanyAuth.Application.Interfaces.Repositories;

public interface IAuthorizationRepository
{
    Task<List<Authorization>> GetByRoleIdAsync(int roleId);
    Task AddAsync(Authorization authorization);
    Task RemoveAsync(Authorization authorization);
    Task SaveChangesAsync();
}
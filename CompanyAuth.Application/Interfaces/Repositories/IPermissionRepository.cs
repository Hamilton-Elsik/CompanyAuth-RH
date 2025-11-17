using CompanyAuth.Domain.Entities;

namespace CompanyAuth.Application.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task AddAsync(Permission permission);
    Task<List<Permission>> GetAllAsync();
    Task RemoveAsync(Permission permission);
    Task SaveChangesAsync();
    Task<Permission?> GetByIdAsync(int id);
}
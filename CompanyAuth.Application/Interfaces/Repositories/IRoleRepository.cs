using CompanyAuth.Domain.Entities;

namespace CompanyAuth.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task AddAsync(Role role);
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task SaveChangesAsync();
}
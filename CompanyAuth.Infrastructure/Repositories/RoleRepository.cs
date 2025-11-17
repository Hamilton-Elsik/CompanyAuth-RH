using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using CompanyAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CompanyAuth.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Role>> GetAllAsync() =>
        _context.Roles.ToListAsync(); 

    public Task<Role?> GetByIdAsync(int id) =>
        _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
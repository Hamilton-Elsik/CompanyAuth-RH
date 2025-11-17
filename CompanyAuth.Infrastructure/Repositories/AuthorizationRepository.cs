using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using CompanyAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CompanyAuth.Infrastructure.Repositories;

public class AuthorizationRepository : IAuthorizationRepository
{
    private readonly AppDbContext _context;

    public AuthorizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Authorization>> GetByRoleIdAsync(int roleId) =>
        _context.Authorizations
            .Where(a => a.RoleId == roleId)
            .Include(a => a.Permission)
            .ToListAsync();

    public async Task AddAsync(Authorization authorization) =>
        await _context.Authorizations.AddAsync(authorization);

    public Task RemoveAsync(Authorization authorization)
    {
        _context.Authorizations.Remove(authorization);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using CompanyAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CompanyAuth.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        // READ ALL
        public Task<List<Permission>> GetAllAsync() =>
            _context.Permissions.ToListAsync();

        // READ BY ID
        public Task<Permission?> GetByIdAsync(int id) =>
            _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);

        // DELETE
        public Task RemoveAsync(Permission permission)
        {
            _context.Permissions.Remove(permission);
            return Task.CompletedTask;
        }

        // SAVE CHANGES
        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();
    }
}

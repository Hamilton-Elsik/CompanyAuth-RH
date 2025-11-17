using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using CompanyAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CompanyAuth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.Include(u => u.Role)
                      .FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(int id) =>
        _context.Users.Include(u => u.Role)
                      .FirstOrDefaultAsync(u => u.Id == id);

    public Task<List<User>> GetAllAsync() =>
        _context.Users.Include(u => u.Role).ToListAsync();

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task RemoveAsync(User user)   
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }
}
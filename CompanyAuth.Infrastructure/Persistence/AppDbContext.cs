using CompanyAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyAuth.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Authorization> Authorizations => Set<Authorization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.HasOne(u => u.Role)
             .WithMany(r => r.Users)
             .HasForeignKey(u => u.RoleId);
            b.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).ValueGeneratedOnAdd();
            b.Property(r => r.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Permission>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Authorization>(b =>
        {
            b.HasKey(a => a.Id);
            b.HasOne(a => a.Role)
             .WithMany(r => r.Authorizations)
             .HasForeignKey(a => a.RoleId);

            b.HasOne(a => a.Permission)
             .WithMany(p => p.Authorizations)
             .HasForeignKey(a => a.PermissionId);
        });
    }
}
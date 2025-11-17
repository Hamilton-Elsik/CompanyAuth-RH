using CompanyAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyAuth.Infrastructure.Persistence.Seed;

public static class InitialDataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Aplicar migraciones automáticamente (opcional pero recomendado)
        await context.Database.MigrateAsync();

        // ============================
        // SEED ROLES
        // ============================
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Id = 1, Name = "Admin", Description = "Administrador del sistema" },
                new Role { Id = 2, Name = "Empleado", Description = "Empleado estándar" }
            );
            await context.SaveChangesAsync();
        }

        // ============================
        // SEED PERMISSIONS
        // ============================
        if (!context.Permissions.Any())
        {
            context.Permissions.AddRange(
                new Permission { Id = 1, Name = "ManageUsers", Description = "CRUD usuarios", Module = "Users" },
                new Permission { Id = 2, Name = "ViewUsers", Description = "Ver usuarios", Module = "Users" }
            );
            await context.SaveChangesAsync();
        }

        // ============================
        // SEED AUTHORIZATIONS
        // ============================
        if (!context.Authorizations.Any())
        {
            context.Authorizations.AddRange(
                new Authorization
                {
                    Id = 1,
                    RoleId = 1,       // Admin
                    PermissionId = 1, // ManageUsers
                    DateAuth = DateTime.UtcNow
                },
                new Authorization
                {
                    Id = 2,
                    RoleId = 1,       // Admin
                    PermissionId = 2, // ViewUsers
                    DateAuth = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }

        // ============================
        // SEED USER ADMIN (opcional)
        // ============================
        if (!context.Users.Any())
        {
            var admin = new User
            {
                Id = 1,
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@system.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                RoleId = 1
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
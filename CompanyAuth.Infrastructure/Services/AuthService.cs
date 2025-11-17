using CompanyAuth.Application.DTOs.Users;
using CompanyAuth.Application.DTOs;
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Application.Interfaces.Services;
using CompanyAuth.Domain.Entities;
using CompanyAuth.Infrastructure.Security;

namespace CompanyAuth.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtGenerator _jwt;

    public AuthService(IUserRepository users, IJwtGenerator jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public async Task<LoginResponse?> ValidateCredentialsAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user is null) return null;

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        var userDto = new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role.Name);
        var token = _jwt.GenerateToken(user, user.Role);

        return new LoginResponse(token, userDto);
    }

    public async Task<UserDto> RegisterUserAsync(RegisterUserRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            RoleId = request.RoleId,
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();


        var created = await _users.GetByIdAsync(user.Id);

        return new UserDto(
            created!.Id,
            created.FirstName,
            created.LastName,
            created.Email,
            created.Role.Name
        );
    }
}
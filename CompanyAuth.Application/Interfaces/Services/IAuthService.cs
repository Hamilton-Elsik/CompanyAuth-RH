

using CompanyAuth.Application.DTOs;
using CompanyAuth.Application.DTOs.Users;

namespace CompanyAuth.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse?> ValidateCredentialsAsync(LoginRequest request);
    Task<UserDto> RegisterUserAsync(RegisterUserRequest request);
}
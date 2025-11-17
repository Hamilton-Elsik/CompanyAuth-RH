using CompanyAuth.Application.DTOs.Users;

namespace CompanyAuth.Application.DTOs;

public record LoginResponse(string Token, UserDto User);
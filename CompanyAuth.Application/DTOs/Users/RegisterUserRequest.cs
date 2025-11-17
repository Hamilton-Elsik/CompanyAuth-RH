namespace CompanyAuth.Application.DTOs.Users;

public record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    int RoleId
    );
namespace CompanyAuth.Application.DTOs.Permissions;

public record PermissionDto(
    int Id,
    string Name,
    string Description,
    string Module
);
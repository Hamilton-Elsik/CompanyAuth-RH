namespace CompanyAuth.Application.DTOs.Permissions;

public class CreatePermissionRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Module { get; set; } = null!;
}

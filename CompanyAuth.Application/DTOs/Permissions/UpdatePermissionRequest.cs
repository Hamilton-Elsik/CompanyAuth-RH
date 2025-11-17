namespace CompanyAuth.Application.DTOs.Permissions;

public class UpdatePermissionRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Module { get; set; } = null!;
}
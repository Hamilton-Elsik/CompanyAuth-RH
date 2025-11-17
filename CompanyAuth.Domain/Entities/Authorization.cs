namespace CompanyAuth.Domain.Entities;

public class Authorization
{
    public int Id { get; set; }
    public DateTime DateAuth { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
namespace CompanyAuth.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Module { get; set; } = null!;

    public ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();
}
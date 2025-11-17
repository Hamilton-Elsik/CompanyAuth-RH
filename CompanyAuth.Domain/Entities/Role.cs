namespace CompanyAuth.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();
    public ICollection<User> Users { get; set; } = new List<User>();
}

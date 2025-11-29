namespace Friendshub.Domain.Models;

public class Role
{
    // TODO: Use Consistent Id across all entities
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

}

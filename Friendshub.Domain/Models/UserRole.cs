namespace Friendshub.Domain.Models
{
    public class UserRole
    {
        public int RoleId { get; set; }
        public Guid UserId { get; set; }
        public Role Role { get; set; }
        public User User { get; set; }
    }
}

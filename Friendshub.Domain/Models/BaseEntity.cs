namespace Friendshub.Domain.Models
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}

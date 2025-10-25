namespace Friendshub.Domain.Models
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}

namespace Friendshub.Domain.Models
{
    public class PostImage
    {
        public Guid Id { get; set; }
        public string ImgUrl { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }

    }
}

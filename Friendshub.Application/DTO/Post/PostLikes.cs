using Friendshub.Application.DTO.User;
namespace Friendshub.Application.DTO.Post
{
    public class PostLikes
    {
        public int Count { get; set; }
        public List<UserBasicInfo> Users { get; set; }
    }
}

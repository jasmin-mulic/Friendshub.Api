using Friendshub.Application.Features.Users.DTO;
namespace Friendshub.Application.Features.Posts.DTO
{
    public class PostLikes
    {
        public int Count { get; set; }
        public List<UserBasicInfo> Users { get; set; } = new();
    }
}

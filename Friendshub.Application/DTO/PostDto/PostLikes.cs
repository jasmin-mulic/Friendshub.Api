using Friendshub.Application.DTO.UserDto;
namespace Friendshub.Application.DTO.DtoPost
{
    public class PostLikes
    {
        public int Count { get; set; }
        public List<UserBasicInfo> Users { get; set; } = new();
    }
}

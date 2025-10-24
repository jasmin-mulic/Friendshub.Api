using Friendshub.Application.DTO.DtoPost;

namespace Friendshub.Application.DTO.UserDto
{
    public class UserProfileData
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool PrivateAccount { get; set; }
        public List<PostClientDto> Posts { get; set; } = new List<PostClientDto>();
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}

using Friendshub.Application.Features.Posts.DTO;

namespace Friendshub.Application.Features.Users.DTO
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
        public int PostCount { get; set; }
    }
}

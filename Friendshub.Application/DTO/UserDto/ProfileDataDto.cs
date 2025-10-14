namespace Friendshub.Application.DTO.UserDto
{
    public class ProfileDataDto
    {
        public string DisplayUsername { get; set; }
        public string ProfileImgUrl { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostCount { get; set; }
        public bool PrivateAccount { get; set; }
    }
}

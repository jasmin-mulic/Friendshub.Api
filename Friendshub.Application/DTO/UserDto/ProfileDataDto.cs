namespace Friendshub.Application.DTO.UserDto
{
    public class ProfileDataDto
    {
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostCount { get; set; }
        public bool PrivateAccount { get; set; }
        public string EmailAddress { get; set; }
    }
}

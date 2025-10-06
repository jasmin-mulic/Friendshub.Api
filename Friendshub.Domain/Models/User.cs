namespace Friendshub.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string DisplayUsername { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public bool Active { get; set; } = true;
        public string ProfileImgUrl { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public List<UserRole> UserRoles { get; set; } = new();  
        public List<Post> Posts { get; set;} = new();
        public List<Follows> Followers { get; set; } = new();
        public List<Follows> Followings { get; set; } = new();
        public List<PostLike> Likes { get; set; } = new();
        public List<Comment> Comments { get; set; }= new();
    }
}

 
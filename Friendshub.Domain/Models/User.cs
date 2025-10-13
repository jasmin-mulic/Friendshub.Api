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
        public string? ProfileImgUrl { get; set; } = null;
        public string PasswordHash { get; set; } = string.Empty;
        public List<UserRole> UserRoles { get; set; } = new();  
        public List<Post> Posts { get; set;} = new();
        public List<Follows> Followers { get; set; } = new();
        public List<Follows> Followings { get; set; } = new();
        public List<PostLike> PostLikes { get; set; } = new();
        public List<CommentLike> CommentLikes { get; set; } = new();
        public bool PrivateAccount { get; set; } = false;
        public List<FollowRequest> SentFollowRequests { get; set; } = new();
        public List<FollowRequest> RecievedFollowRequest { get; set; } = new();
     }

}

 
namespace Friendshub.Application.DTO.DtoPost
{
    public class CommentClientDto
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Username { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        public string CommentImageUrl { get; set; }
    }
}

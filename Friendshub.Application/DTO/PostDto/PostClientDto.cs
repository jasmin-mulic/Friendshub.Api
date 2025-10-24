using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.DTO.DtoPost
{
    public class PostClientDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime PostedAt { get; set; }
        public List<string> PostImagesUrl { get; set; } = new List<string>();
        public string Username { get; set; }
        public string ProfileImgUrl { get; set; }
        public List<UserBasicInfo> Likes { get; set; }  = new List<UserBasicInfo>();
        public List<CommentClientDto> Comments { get; set; } = new List<CommentClientDto>();
        public int LikeCount  { get; set; }
    }
}

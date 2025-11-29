using Microsoft.AspNetCore.Http;

namespace Friendshub.Application.Features.Posts.DTO
{
    public class AddCommentDto
    {
        public string Content { get; set; } 
        public IFormFile Image { get; set; }
    }
}

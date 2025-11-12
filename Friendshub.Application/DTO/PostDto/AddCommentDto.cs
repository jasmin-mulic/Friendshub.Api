using Microsoft.AspNetCore.Http;

namespace Friendshub.Application.DTO.PostDto
{
    public class AddCommentDto
    {
        public string Content { get; set; } 
        public IFormFile Image { get; set; }
    }
}

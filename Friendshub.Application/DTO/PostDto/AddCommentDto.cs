using Microsoft.AspNetCore.Http;

namespace Friendshub.Application.DTO.PostDto
{
    public class AddCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
    }
}

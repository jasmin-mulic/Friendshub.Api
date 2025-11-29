using Microsoft.AspNetCore.Http;

namespace Friendshub.Application.Features.Posts.DTO
{
    public class AddPostDto
    {
        public string Content { get; set; }
        public List<IFormFile> ImagePaths { get; set; } = new List<IFormFile>();
    }
}

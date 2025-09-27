using Microsoft.AspNetCore.Http;

namespace Friendshub.Application.DTO.Post
{
    public class AddPostDto
    {
        public string Content { get; set; }
        public List<IFormFile> PostImagesUrls { get; set; }
    }
}

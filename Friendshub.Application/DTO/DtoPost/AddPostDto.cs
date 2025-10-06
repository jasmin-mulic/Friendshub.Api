using Microsoft.AspNetCore.Http;

namespace Friendshub.Application.DTO.DtoPost
{
    public class AddPostDto
    {
        public string Content { get; set; }
        public List<IFormFile> ImagePaths { get; set; } = new List<IFormFile>();
    }
}

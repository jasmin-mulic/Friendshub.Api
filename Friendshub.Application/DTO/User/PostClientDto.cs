using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.DTO.User
{
    public class PostClientDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> PostImagesUrl { get; set; } = new List<string>();
        public int LikeCounter { get; set; }
    }
}

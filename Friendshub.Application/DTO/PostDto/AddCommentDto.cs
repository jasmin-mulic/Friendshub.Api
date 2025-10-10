using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.DTO.PostDto
{
    public class AddCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
    }
}

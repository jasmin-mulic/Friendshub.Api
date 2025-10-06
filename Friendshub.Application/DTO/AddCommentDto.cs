using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.DTO
{
    public class AddCommentDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public string CommentImageUrl { get; set; }
    }
}

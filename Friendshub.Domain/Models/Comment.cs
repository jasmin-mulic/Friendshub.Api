using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Domain.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid PostId { get; set; }
        public string ImgUrl { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public DateTime CommentedAt { get; set; }





    }
}

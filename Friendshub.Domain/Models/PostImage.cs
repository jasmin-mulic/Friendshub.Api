using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Domain.Models
{
    public class PostImage
    {
        public Guid Id { get; set; }
        public string ImgUrl { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }

    }
}

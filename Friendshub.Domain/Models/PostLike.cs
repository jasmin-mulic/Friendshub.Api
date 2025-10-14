using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Domain.Models
{
    public class PostLike
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public DateTime LikedAt { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}

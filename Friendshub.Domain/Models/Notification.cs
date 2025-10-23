using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Domain.Models
{
    internal class Notification
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecieverId { get; set; }
    }
}

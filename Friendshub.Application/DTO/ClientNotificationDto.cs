using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.DTO
{
    public class ClientNotificationDto
    {
        public Guid Id { get; set; }
        public string SenderUsername { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; }
        public string SenderProfileImageUrl { get; set; }

    }
}

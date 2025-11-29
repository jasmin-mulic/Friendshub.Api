using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Features.Users.DTO
{
    public class UserBasicInfo
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}

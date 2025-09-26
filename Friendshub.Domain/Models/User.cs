using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string DisplayUsername { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public bool Active { get; set; } = true;
        public string ProfileImgUrl { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();    
        public ICollection<Post> Posts { get; set;} = new List<Post>();
        public List<Follows> Followers { get; set; } = new();
        public List<Follows> Followings { get; set; } = new();
    }
}

 
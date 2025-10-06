using Friendshub.Domain.Models;

namespace Friendshub.Application.DTO.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public User User { get; set; }
    }
}

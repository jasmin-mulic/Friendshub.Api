using Friendshub.Domain.Models;

namespace Friendshub.Application.Results
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public User  User { get; set; }

    }
}

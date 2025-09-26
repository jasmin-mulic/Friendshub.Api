using Friendshub.Application.DTO;
namespace Friendshub.Application.Results
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public List<RegisterUserError> ValidationErrors { get; set; } = new List<RegisterUserError>();
        public Guid UserId { get; set; } = Guid.Empty;
    }
}

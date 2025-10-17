namespace Friendshub.Application.DTO.UserDto
{
    public class UpdateUserValidationDto
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; } = null;

    }
}

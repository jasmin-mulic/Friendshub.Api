namespace Friendshub.Application.DTO.UserDto
{
    public class FollowRecommendationDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool PendingRequest {  get; set; }
    }
}

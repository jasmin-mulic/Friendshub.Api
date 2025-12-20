using Friendshub.Api.Extensions;
using Friendshub.Application.Features.Posts;
using Friendshub.Application.Features.Posts.DTO;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friendshub.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ILIkeService _likeService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly INotificationService _notificationService;
        public PostsController(IPostService postService, ILIkeService likeService, ICommentService commentService, INotificationService notificationService)
        {
            _postService = postService;
            _likeService = likeService;
            _commentService = commentService;
            _notificationService = notificationService;
        }
        [HttpPost()]
        public async Task<IActionResult> AddPost(AddPostDto request)
        {
            try
            {
                var UserIdFromClaims = User.GetUserId();
                if (UserIdFromClaims == Guid.Empty)
                    return Unauthorized("You are logged out.");

                if (string.IsNullOrWhiteSpace(request.Content) && (request.ImagePaths == null || request.ImagePaths.Any()))
                    return BadRequest(new { message = "You have to add at least one image or post content." });
                var newPost = await _postService.AddPost(request, UserIdFromClaims);
                return Ok(newPost);

            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetMyPosts([FromQuery] int page = 1)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("You are logged out.");
            }

            var posts = await _postService.GetLoggedUserPosts(userId, page);
            return Ok(posts);
        }


        [HttpGet("feed")]
        public async Task<IActionResult> GetFeedPosts([FromQuery] int page = 1)
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized("You are logged out.");

                var feed = await _postService.GetFeedPosts(userIdFromClaims, page);

                return Ok(feed);

            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);

            }
        }
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute]Guid postId)
        {
            try
            {
                if (User.GetUserId() == Guid.Empty)
                    return Unauthorized("You are logged out.");

               var isDeleted = await  _postService.DeletePost(postId);
                if (!isDeleted)
                    return BadRequest("Error deleting post");

                return Ok(new { message = "Post deleted successfully." });
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
        
        [HttpPost("{postId}/likes")]
        public async Task<IActionResult> LikePost([FromRoute]Guid postId)
        {
            try
            {
                var userId = User.GetUserId();

                 if (userId == Guid.Empty)
                return Unauthorized("You are not logged in.");

                var post = await _postService.GetPostByIdAsync(postId);
                if (post == null)
                    return BadRequest("Post is deleted.");

                var isLiked = await _likeService.LikePost(userId, postId);
                await _notificationService.CreateNotification(userId, post.UserId, NotificationType.Like, postId);
                return Ok(new {IsLiked = isLiked});
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
        
        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> AddCommentToPost([FromRoute] Guid postId, AddCommentDto comment)
        {
            try
            {
                var userIdFromClaIms = User.GetUserId();

                if (userIdFromClaIms == Guid.Empty)
                    return Unauthorized();

                var post = await _postService.GetPostByIdAsync(postId);

                if(post == null)
                    return NotFound("Post is deleted");

                var newComment = await _commentService.AddComment(userIdFromClaIms, post, comment);
                if (newComment == null)
                    return BadRequest("Error adding post.");
                return Ok(newComment);  
                
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
        
        [HttpPost("comments/{commentId}/likes")]
        public async Task<IActionResult> LikeComment([FromRoute] Guid commentId)
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if(userIdFromClaims == Guid.Empty)
                    return Unauthorized("You are logged out.");
                var likeResponse = await _likeService.LikePostComment(commentId, userIdFromClaims);
                return Ok(new { likeResponse});
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
        
        [HttpDelete("comment/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId)
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if (userIdFromClaims == Guid.Empty)
                    return Unauthorized("You are logged out");
                await _commentService.RemoveComment(commentId);
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

    }
}

using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.Implementations;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
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
        public PostsController(IPostService postService, ILIkeService likeService, ICommentService commentService)
        {
            _postService = postService;
            _likeService = likeService;
            _commentService = commentService;
        }
        [HttpGet("my-posts/page/{page}")]
        public async Task<IActionResult> GetmyPosts([FromRoute] int page)
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized("You are logged out.");
                var posts = await _postService.GetLoggedUserPosts(userIdFromClaims, page);
                return Ok(posts);
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }
        [HttpPost("add-post")]
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
        [HttpGet("get-feed-posts/page/{page}")]
        public async Task<IActionResult> GetFeedPosts([FromRoute] int page)
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
        [HttpDelete("delete-post")]
        public async Task<IActionResult> DeletePost(Guid postId)
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
        [HttpPost("like")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            try
            {
                var userId = User.GetUserId();

                 if (userId == Guid.Empty)
                return Unauthorized("Session expired. Please log in.");

                var post = await _postService.GetPostByIdAsync(postId);
                if (post == null)
                    return BadRequest("Post is deleted.");

                var likeResponse = await _likeService.LikePost(userId, postId);
                return Ok(likeResponse);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
        
        [HttpPost("add-comment/{postId}")]
        public async Task<IActionResult> AddComment([FromRoute] Guid postId, AddCommentDto comment)
        {
            try
            {
                var userIdFromClaIms = User.GetUserId();

                if (userIdFromClaIms == Guid.Empty)
                    return Unauthorized();

                var post = await _postService.GetPostByIdAsync(postId);

                if(post == null)
                    return NotFound("Post is deleted");

                var newComment = await _commentService.AddCommentToPost(userIdFromClaIms, post, comment);
                if (newComment == null)
                    return BadRequest("Error adding post.");
                return Ok(newComment);  
                
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
        [HttpPost("like-comment/{commentId}")]
        public async Task<IActionResult> LikeComment([FromRoute] Guid commentId)
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if(userIdFromClaims == Guid.Empty)
                    return Unauthorized("You are logged out.");
                var likeResponse = await _likeService.LikePostComment(commentId, userIdFromClaims);
                return Ok(likeResponse);
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

using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.Post;
using Friendshub.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Friendshub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpGet("my-posts")]
        public async Task<IActionResult> GetmyPosts()
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized("You are logged out.");
                var posts = await _unitOfWork.PostRepository.GetMyPosts(userIdFromClaims);
                return Ok(posts);
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }
        [Authorize]
        [HttpPost("add-post")]
        public async Task<IActionResult> AddPost(AddPostDto request)
        {
            try
            {
                var UserIdFromClaims = User.GetUserId();
                if (UserIdFromClaims == Guid.Empty)
                    return Unauthorized("You are logged out.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if(request.ImagePaths.Count > 0)
                {
                foreach (var image in request.ImagePaths)
                {
                    var extension = Path.GetExtension(image.FileName.ToLowerInvariant());
                    if (!allowedExtensions.Contains(extension))
                        return BadRequest("Image format not supported!");

                        long maxSize = 10 * 1024 * 1024;
                    if (image.Length > maxSize)
                        return BadRequest("Image exceedes 10 MB!.");
                }
                }

                if (string.IsNullOrWhiteSpace(request.Content) && request.ImagePaths == null)
                    return BadRequest(new { message = "You have to add at least one image or post content." });

                var newPost = await _unitOfWork.PostRepository.AddPost(request, UserIdFromClaims);
                await _unitOfWork.ApplyChanges();
                return Ok(new { message = "Post added successfully" });

            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }
        [Authorize]
        [HttpGet("get-feed-posts")]
        public async Task<IActionResult>GetFeedPosts()
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized("You are logged out.");

                var feed = await _unitOfWork.PostRepository.GetFeedPosts(userIdFromClaims);
                return Ok(feed);
                
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);

            }
        }
        [HttpPost("delete-post")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                if (User.GetUserId() == Guid.Empty)
                    return Unauthorized("You are logged out.");
                var post = await _unitOfWork.PostRepository.GetPostById(postId);
                if(post == null)
                    return NotFound("Post not found.");
                _unitOfWork.PostRepository.DeletePost(post);
                await _unitOfWork.ApplyChanges();
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

                var post = await _unitOfWork.PostRepository.GetPostById(postId);
                if (post == null)
                    return BadRequest("Post is deleted.");
                var likeResponse = await _unitOfWork.PostRepository.LikePost(userId, postId);
                await _unitOfWork.ApplyChanges();
                return Ok(likeResponse);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}

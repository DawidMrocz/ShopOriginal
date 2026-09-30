using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using ProductApp.ApiModels.Comment;
using ProductApp.Services.Comment;

namespace ProductApp.Controllers
{
    [Route("[controller]")]
    public class CommentController : AuthorizedController
    {
        private readonly ICommentService _commentService;
        public CommentController(IServiceProvider serviceProvider, ICommentService commentService) : base(serviceProvider)
        {
            _commentService = commentService;
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> Create([FromRoute] int productId, [FromBody] CommentCreateRequest request)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _commentService.Create(request, productId, Guid.Parse(AuthorizedUser.Id));
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> Update([FromRoute] int commentId, [FromBody] CommentUpdateRequest request)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _commentService.Update(request, commentId, Guid.Parse(AuthorizedUser?.Id));
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete([FromRoute] int commentId)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _commentService.Delete(commentId, Guid.Parse(AuthorizedUser?.Id));
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

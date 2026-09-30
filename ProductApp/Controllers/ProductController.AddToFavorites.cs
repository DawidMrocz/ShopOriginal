using Microsoft.AspNetCore.Mvc;

namespace ProductApp.Controllers
{
    public partial class ProductController
    {
        [HttpPost("{productId}/favorites")]
        public async Task<IActionResult> AddFavorite([FromRoute] int productId)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _productService.AddToFavorites(productId, Guid.Parse(AuthorizedUser.Id),Culture);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

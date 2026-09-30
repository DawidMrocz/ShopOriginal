using Microsoft.AspNetCore.Mvc;

namespace ProductApp.Controllers
{
    public partial class ProductController
    {
        [HttpDelete("{productId}/favorites")]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int productId)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _productService.DeleteToFavorites(productId, Guid.Parse(AuthorizedUser.Id));
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

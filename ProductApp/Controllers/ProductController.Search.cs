using Microsoft.AspNetCore.Mvc;
using ProductApp.ApiModels.Product;

namespace ProductApp.Controllers
{
    public partial class ProductController
    {
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request)
        {
            try
            {
                return Ok(await _productService.Search(request, Guid.Parse(AuthorizedUser?.Id),Culture));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}

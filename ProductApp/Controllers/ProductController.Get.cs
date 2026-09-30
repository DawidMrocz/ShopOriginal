using Microsoft.AspNetCore.Mvc;

namespace ProductApp.Controllers
{
    public partial class ProductController
    {
        [HttpGet("{productId}")]
        public async Task<IActionResult> Get([FromRoute] int productId)
        {
            try
            {
                return Ok(await _productService.Get(productId,Guid.Parse(AuthorizedUser?.Id),Culture));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

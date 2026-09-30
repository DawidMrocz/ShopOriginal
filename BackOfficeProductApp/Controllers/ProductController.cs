using BackOfficeProductApp.Services.ProductService;
using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using BackOfficeProductApp.ApiModels.Product;

namespace BackOfficeProductApp.Controllers
{
    [Route("[controller]")]
    public partial class ProductController : AuthorizedController
    {
        private readonly IProductService _productService;

        public ProductController(IServiceProvider serviceProvider, IProductService productService) : base(serviceProvider)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request)
        {
            try
            {
                return Ok(await _productService.Search(request, Culture));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Get([FromRoute] int productId)
        {
            try
            {
                return Ok(await _productService.Get(productId, Guid.Parse(AuthorizedUser?.Id), Culture));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _productService.Create(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("/{productId}")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _productService.Delete(productId);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromRoute] int productId)
        {
            try
            {
                return Ok(await _productService.Statistics());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductUpdateRequest request)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _productService.Update(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

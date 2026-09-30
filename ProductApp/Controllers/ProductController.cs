using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using ProductApp.Services.ProductService;

namespace ProductApp.Controllers
{
    [Route("[controller]")]
    public partial class ProductController : AuthorizedController
    {
        private readonly IProductService _productService;

        public ProductController(IServiceProvider serviceProvider, IProductService productService) : base(serviceProvider)
        {
            _productService = productService;
        }
    }
}

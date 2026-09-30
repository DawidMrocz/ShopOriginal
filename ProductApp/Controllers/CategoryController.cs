using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using ProductApp.Services.Category;

namespace ProductApp.Controllers
{
    [Route("[controller]")]
    public partial class CategoryController : AuthorizedController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(IServiceProvider serviceProvider, ICategoryService categoryService) : base(serviceProvider)
        {
            _categoryService = categoryService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _categoryService.GetList());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

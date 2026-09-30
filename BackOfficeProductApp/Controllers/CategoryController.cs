using BackOfficeProductApp.Services.Category;
using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeProductApp.Controllers
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

        [HttpPost("categories")]
        public async Task<IActionResult> Create([FromBody] string request)
        {
            try
            {
                await _categoryService.Create(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("categories/{categoryId}")]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
            try
            {
                await _categoryService.Delete(categoryId);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

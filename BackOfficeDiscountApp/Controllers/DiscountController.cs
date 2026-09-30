using BackOfficeDiscountApp.ApiModels;
using BackOfficeDiscountApp.Services;
using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeDiscountApp.Controllers
{
    public class DiscountController : AuthorizedController
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IServiceProvider serviceProvider, IDiscountService discountService): base(serviceProvider)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] DiscountSearchRequest request)
        {
            try
            {
                return Ok(await _discountService.Search(request, Guid.Parse(AuthorizedUser?.Id), Culture));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{discountId}")]
        public async Task<IActionResult> Get([FromRoute] int discountId)
        {
            try
            {
                return Ok(await _discountService.Get(discountId, Guid.Parse(AuthorizedUser?.Id), Culture));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DiscountCreateRequest request)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _discountService.Create(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("/{discountId}")]
        public async Task<IActionResult> Delete([FromRoute] int discountId)
        {
            try
            {
                if (AuthorizedUser is null) return Unauthorized();
                await _discountService.Delete(discountId);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

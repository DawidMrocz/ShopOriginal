using Microsoft.AspNetCore.Mvc;

namespace BasketApp.Controllers
{
    public partial class BasketController
    {
        [HttpGet("baskets")]
        public async Task<IActionResult> Get()
        {
            if (AuthorizedUser is null) return Unauthorized();
            return Ok(await _basketService.Get(Guid.Parse(AuthorizedUser.Id),Culture));
        }
    }
}

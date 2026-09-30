using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasketApp.Controllers
{
    public partial class BasketItemController
    {
        [HttpDelete("basket-items/{basketItemId}")]
        public async Task<IActionResult> Delete([FromRoute] int basketItemId, [FromBody] int quantity)
        {
            if (AuthorizedUser is null) return Unauthorized();

            await _basketItemService.Delete(basketItemId, Guid.Parse(AuthorizedUser.Id));
            return Ok("Pomyślnie dodano zniżkę");
        }
    }
}

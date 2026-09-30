using Microsoft.AspNetCore.Mvc;

namespace BasketApp.Controllers
{
    public partial class BasketItemController
    {
        [HttpPut("basket-items/{basketItemId}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int basketItemId, [FromBody] int quantity)
        {
            if (AuthorizedUser is null) return Unauthorized();

            await _basketItemService.UpdateQuantity(basketItemId, quantity, Guid.Parse(AuthorizedUser.Id));
            return Ok("Pomyślnie dodano zniżkę");
        }
    }
}

using BasketApp.Data;
using BasketApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Services.BasketItem
{
    public class BasketItemService : IBasketItemService
    {
        private readonly BasketContext _basketContext;

        public BasketItemService(BasketContext basketContext)
        {
            _basketContext = basketContext;
        }
      
        public async Task Delete(int basketItemId, Guid userId)
        {
            User user = await _basketContext.Users.Include(b => b.Basket).ThenInclude(i => i.BasketItems).FirstOrDefaultAsync(u => u.ExternalId == userId)
               ?? throw new Exception("User not found");

            Models.BasketItem item = user.Basket.BasketItems.FirstOrDefault(i => i.Id == basketItemId)
            ?? throw new Exception("Basket item not found");

            _basketContext.Remove(item);

            if (item.Basket.UserId != userId) throw new Exception("You cannot modify somebody's basket");

            await _basketContext.SaveChangesAsync();
        }

        public async Task UpdateQuantity(int basketItemId, int quantity, Guid userId)
        {
            User user = await _basketContext.Users.Include(b => b.Basket).ThenInclude(i => i.BasketItems).FirstOrDefaultAsync(u => u.ExternalId == userId)
               ?? throw new Exception("User not found");

            Models.BasketItem basketItem = user.Basket.BasketItems.FirstOrDefault(i => i.Id == basketItemId) ?? throw new Exception("item not found");

            if (basketItem.Basket.UserId != user.Id) throw new Exception("Server error");

            basketItem.Quantity = quantity;

            await _basketContext.SaveChangesAsync();
        }
    }
}

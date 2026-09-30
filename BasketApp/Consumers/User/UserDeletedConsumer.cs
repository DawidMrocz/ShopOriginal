using BasketApp.Data;
using BasketApp.Models;
using Contracts.RabbitMessages.UserApp;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Consumers.User
{
    public class UserDeletedConsumer : IConsumer<UserDeleted>
    {
        private readonly BasketContext _basketContext;

        public UserDeletedConsumer(BasketContext basketDbContext)
        {
            _basketContext = basketDbContext;
        }

        public async Task Consume(ConsumeContext<UserDeleted> context)
        {
            Models.User model = await _basketContext.Users.FirstOrDefaultAsync(u => u.ExternalId == context.Message.Id)
                ?? throw new Exception("User not found");

            //Basket basket = await _basketContext.Baskets.FirstOrDefaultAsync(b => b.UserId == model.Id)
            //    ?? throw new Exception("Basket not found");

            //_basketContext.RemoveRange(basket.BasketItems);
            //_basketContext.Remove(basket);
            _basketContext.Users.Remove(model);

            //await _basketContext.Baskets.AddAsync(basket);
            await _basketContext.SaveChangesAsync();
        }
    }
}

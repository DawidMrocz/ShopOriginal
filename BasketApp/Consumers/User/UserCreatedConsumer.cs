using BasketApp.Data;
using BasketApp.Models;
using Contracts.RabbitMessages.UserApp;
using MassTransit;

namespace BasketApp.Consumers.User
{
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly BasketContext _basketContext;

        public UserCreatedConsumer(BasketContext basketDbContext)
        {
            _basketContext = basketDbContext;
        }

        public async Task Consume(ConsumeContext<UserCreated> context)
        {
            Models.User newUser = new()
            {
                ExternalId = context.Message.Id,
                Email = context.Message.Email,
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                Street = context.Message.Street,
                City = context.Message.City,
                Country = context.Message.Country,
                PostalCode = context.Message.PostalCode,
            };

            await _basketContext.AddAsync(newUser);

            Models.Basket basket = new()
            {
                User = newUser,
            };

            await _basketContext.Baskets.AddAsync(basket);
            await _basketContext.SaveChangesAsync();
        }
    }
}

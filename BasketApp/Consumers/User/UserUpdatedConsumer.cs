using BasketApp.Data;
using Contracts.RabbitMessages.UserApp;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Consumers.User
{
    public class UserUpdatedConsumer : IConsumer<UserUpdated>
    {
        private readonly BasketContext _basketContext;

        public UserUpdatedConsumer(BasketContext basketDbContext)
        {
            _basketContext = basketDbContext;
        }

        public async Task Consume(ConsumeContext<UserUpdated> context)
        {
            Models.User model = await _basketContext.Users.FirstOrDefaultAsync(u => u.ExternalId == context.Message.Id) ?? throw new Exception("User not found");

            model.Email = context.Message.Email;

            await _basketContext.SaveChangesAsync();
        }
    }
}

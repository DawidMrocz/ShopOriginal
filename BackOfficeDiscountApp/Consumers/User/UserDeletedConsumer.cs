using BackOfficeContracts.RabbitMessages.UserApp;
using BackOfficeDiscountApp.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeDiscountApp.Consumers.User
{
    public class UserDeletedConsumer : IConsumer<UserDeleted>
    {
        private readonly DiscountContext _discountContext;

        public UserDeletedConsumer(DiscountContext discountDbContext)
        {
            _discountContext = discountDbContext;
        }

        public async Task Consume(ConsumeContext<UserDeleted> context)
        {
            Models.User model = await _discountContext.Users.FirstOrDefaultAsync(u => u.ExternalId == context.Message.Id)
                ?? throw new Exception("User not found");

            _discountContext.Remove(model);
            await _discountContext.SaveChangesAsync();
        }
    }
}

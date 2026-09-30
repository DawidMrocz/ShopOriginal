using BackOfficeContracts.RabbitMessages.UserApp;
using BackOfficeProductApp.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeProductApp.Consumers.User
{
    public class UserDeletedConsumer : IConsumer<UserDeleted>
    {
        private readonly ProductContext _productContext;

        public UserDeletedConsumer(ProductContext productDbContext)
        {
            _productContext = productDbContext;
        }

        public async Task Consume(ConsumeContext<UserDeleted> context)
        {
            Models.User model = await _productContext.Users.FirstOrDefaultAsync(u => u.ExternalId == context.Message.Id)
                ?? throw new Exception("User not found");

            _productContext.Remove(model);
            await _productContext.SaveChangesAsync();
        }
    }
}

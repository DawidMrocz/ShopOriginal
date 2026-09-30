using BackOfficeContracts.RabbitMessages.UserApp;
using BackOfficeProductApp.Data;
using MassTransit;

namespace BackOfficeProductApp.Consumers.User
{
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly ProductContext _productContext;

        public UserCreatedConsumer(ProductContext productDbContext)
        {
            _productContext = productDbContext;
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

            await _productContext.Users.AddAsync(newUser);
            await _productContext.SaveChangesAsync();
        }
    }
}

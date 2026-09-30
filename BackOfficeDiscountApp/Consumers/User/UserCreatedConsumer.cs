using BackOfficeContracts.RabbitMessages.UserApp;
using BackOfficeDiscountApp.Data;
using MassTransit;

namespace BackOfficeDiscountApp.Consumers.User
{
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly DiscountContext _discountContext;

        public UserCreatedConsumer(DiscountContext discountDbContext)
        {
            _discountContext = discountDbContext;
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

            await _discountContext.Users.AddAsync(newUser);
            await _discountContext.SaveChangesAsync();
        }
    }
}

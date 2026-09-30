using Contracts.RabbitMessages.UserApp;
using MassTransit;
using ProductApp.Data;

namespace ProductApp.Consumers.User
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
            using var transaction = _productContext.Database.BeginTransaction();

            try
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

                Models.Favorite model = new()
                {
                    UserId = newUser.Id,
                    ExternalId = context.Message.FavoriteId,
                };

                await _productContext.Favorities.AddAsync(model);
                await _productContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                // TODO: Handle failure
            }
        }
    }
}

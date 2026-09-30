using Contracts.RabbitMessages.Favorite;
using MassTransit;
using ProductApp.Data;

namespace ProductApp.Consumers.Favorite
{
    public class FavoriteCreatedConsumer : IConsumer<FavoriteCreated>
    {
        private readonly ProductContext _productContext;

        public FavoriteCreatedConsumer(ProductContext productDbContext)
        {
            _productContext = productDbContext;
        }

        public async Task Consume(ConsumeContext<FavoriteCreated> context)
        {
            Models.Favorite model = new()
            {
                UserId = context.Message.UserId,
                ExternalId = context.Message.Id
            };

            await _productContext.Favorities.AddAsync(model);
            await _productContext.SaveChangesAsync();
        }
    }
}

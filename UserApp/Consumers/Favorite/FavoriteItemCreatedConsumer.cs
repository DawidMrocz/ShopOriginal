using Contracts.RabbitMessages.Favorite;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;

namespace UserApp.Consumers.Favorite
{
    public class FavoriteItemCreatedConsumer : IConsumer<FavoriteItemCreated>
    {
        private readonly UserContext _userContext;

        public FavoriteItemCreatedConsumer(UserContext userDbContext)
        {
            _userContext = userDbContext;
        }

        public async Task Consume(ConsumeContext<FavoriteItemCreated> context)
        {

            Models.Favorite model = await _userContext.Favorites.FirstOrDefaultAsync(f => f.Id == context.Message.FavoriteId)
                ?? throw new Exception("Favorite not found");

            Models.Product product = await _userContext.Products.FirstOrDefaultAsync(f => f.Id == context.Message.ProductId)
                ?? throw new Exception("Product not found");

            FavoriteItem item = new()
            {
                FavoriteId = model.Id,
                ProductId = product.Id
            };

            model.Items.Add(item);

            await _userContext.SaveChangesAsync();
        }
    }
}

using Contracts.RabbitMessages.Favorite;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;

namespace UserApp.Consumers.Favorite
{
    public class FavoriteItemDeletedConsumer : IConsumer<FavoriteItemDeleted>
    {
        private readonly UserContext _userContext;

        public FavoriteItemDeletedConsumer(UserContext userDbContext)
        {
            _userContext = userDbContext;
        }

        public async Task Consume(ConsumeContext<FavoriteItemDeleted> context)
        {

            FavoriteItem model = await _userContext.FavoriteItems.FirstOrDefaultAsync(f => f.Id == context.Message.Id)
                ?? throw new Exception("Favorite not found");

            _userContext.FavoriteItems.Remove(model);

            await _userContext.SaveChangesAsync();
        }
    }
}

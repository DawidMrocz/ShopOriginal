using Contracts.RabbitMessages.Favorite;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApp.Data;

namespace ProductApp.Consumers.Favorite
{
    public class FavoriteDeletedConsumer : IConsumer<FavoriteDeleted>
    {
        private readonly ProductContext _productContext;

        public FavoriteDeletedConsumer(ProductContext productDbContext)
        {
            _productContext = productDbContext;
        }

        public async Task Consume(ConsumeContext<FavoriteDeleted> context)
        {
            Console.WriteLine("Czy potzrebne");


            Models.Favorite model = await _productContext.Favorities.FirstOrDefaultAsync(u => u.Id == context.Message.Id)
                ?? throw new Exception("Favorite not found");

            _productContext.Remove(model);
            await _productContext.SaveChangesAsync();
        }
    }
}

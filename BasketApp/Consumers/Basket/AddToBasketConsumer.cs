using BasketApp.Data;
using BasketApp.Models;
using Contracts.RabbitMessages.Basket;
using MassTransit;
using Microsoft.EntityFrameworkCore;


namespace BasketApp.Consumers.Basket
{
    public class AddToBasketConsumer : IConsumer<BasketItemCreated>
    {
        private readonly BasketContext _basketContext;
        private readonly ProductAppGrpc.Product.ProductClient _productClient;

        public AddToBasketConsumer(BasketContext basketDbContext, ProductAppGrpc.Product.ProductClient productClient)
        {
            _basketContext = basketDbContext;
            _productClient = productClient;
        }

        public async Task Consume(ConsumeContext<BasketItemCreated> context)
        {
            Models.User user = await _basketContext.Users.Include(b => b.Basket).ThenInclude(i => i.BasketItems).FirstOrDefaultAsync(b => b.ExternalId == context.Message.UserId)
                ?? throw new Exception("Basket not found");

            BasketItem? item = user.Basket.BasketItems.FirstOrDefault(i => i.ProductId == context.Message.ProductId);

            if (item is null)
            {              
                BasketItem newItem = new()
                {
                    BasketId = user.Basket.Id,
                    Quantity = context.Message.Quantity,
                    ProductId = context.Message.ProductId,
                };
                await _basketContext.AddAsync(newItem);
            }
            else
            {
                item.Quantity = ++context.Message.Quantity;
            }

            _productClient.ChangeQuantity(new ProductAppGrpc.ProductChangeQuantityRequest()
            {
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity
            });

            await _basketContext.SaveChangesAsync();           
        }
    }
}

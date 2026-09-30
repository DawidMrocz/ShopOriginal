using Contracts.RabbitMessages.Basket;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApp.Data;

namespace ProductApp.Consumers.Basket
{
    public class BasketItemQuantityUpdatedConsumer : IConsumer<BasketItemUpdatedQuantity>
    {
        private readonly ProductContext _productContext;

        public BasketItemQuantityUpdatedConsumer(ProductContext productDbContext)
        {
            _productContext = productDbContext;
        }

        public async Task Consume(ConsumeContext<BasketItemUpdatedQuantity> context)
        {
            //Models.Product product = await _productContext.Products.FirstOrDefaultAsync(p => p.Id == context.Message.ProductId)
            //    ?? throw new Exception("Product not found");

            //product.Quantity = --context.Message.Quantity;

            await _productContext.SaveChangesAsync();
        }
    }
}

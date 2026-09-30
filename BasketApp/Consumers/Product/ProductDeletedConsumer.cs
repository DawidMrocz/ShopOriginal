using BasketApp.Data;
using Contracts.RabbitMessages.Product;
using Framework.Repositories.File;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BasketApp.Consumers.Product
{
    public class ProductDeletedConsumer : IConsumer<ProductDeleted>
    {
        private readonly BasketContext _basketDbContext;
        private readonly IFileRepository _fileRepository;

        public ProductDeletedConsumer(BasketContext basketDbContext, IFileRepository fileRepository)
        {
            _basketDbContext = basketDbContext;
            _fileRepository = fileRepository;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            //Models.Product model = await _basketDbContext.Products.FirstOrDefaultAsync(p => p.Id == context.Message.Id)
            //    ?? throw new Exception("Product not found");

            //await basketDbContext.BasketItems.Where(b => b.ProductId == model.Id).ExecuteDeleteAsync();
            //await _fileRepository.Delete(model.Photo);
            //_basketDbContext.Products.Remove(model);
            await _basketDbContext.SaveChangesAsync();
        }
    }
}

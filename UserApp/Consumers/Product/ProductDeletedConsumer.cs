using UserApp.Data;
using Contracts.RabbitMessages.Product;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Framework.Repositories.File;

namespace UserApp.Consumers.Product
{
    public class ProductDeletedConsumer : IConsumer<ProductDeleted>
    {
        private readonly IFileRepository _fileRepository;
        private readonly UserContext _userDbContext;

        public ProductDeletedConsumer(IFileRepository fileRepository, UserContext userDbContext)
        {
            _fileRepository = fileRepository;
            _userDbContext = userDbContext;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            Models.Product model = await _userDbContext.Products.FirstOrDefaultAsync(p => p.Id == context.Message.Id)
                ?? throw new Exception("Product not found");

            await _fileRepository.Delete(model.Photo);

            _userDbContext.Remove(model);
            await _userDbContext.SaveChangesAsync();
        }
    }
}

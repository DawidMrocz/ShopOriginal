using Framework.Models.File;

namespace Contracts.RabbitMessages.Product
{
    public class ProductUpdated
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int ProducerId { get; set; }
        public List<int> CategoriesIds { get; set; } = [];
        public FileModel File { get; set; } = null!;
    }
}

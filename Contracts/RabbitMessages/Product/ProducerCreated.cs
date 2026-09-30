using Framework.Models.File;

namespace Contracts.RabbitMessages.Product
{
    public class ProducerCreated
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public FileModel Logo { get; set; } = null!;
    }
}

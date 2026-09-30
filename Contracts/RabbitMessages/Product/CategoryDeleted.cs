namespace Contracts.RabbitMessages.Product
{
    public class CategoryDeleted
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

namespace Contracts.RabbitMessages.Product
{
    public class CategoryCreated
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

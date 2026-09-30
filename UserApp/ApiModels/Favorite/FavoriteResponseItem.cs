using Contracts.RabbitMessages;

namespace UserApp.ApiModels.Favorite
{
    public class FavoriteResponseItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public Price Price { get; set; } = null!;
        public string ProducerName { get; set; } = null!;
        public byte[] ProducerLogo { get; set; } = null!;
    }
}

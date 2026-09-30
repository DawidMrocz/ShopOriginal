using BasketApp.Models;
using Contracts.RabbitMessages;

namespace BasketApp.ApiModels
{
    public class BasketItemResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Price Price { get; set; } = null!;
        public DiscountedPrice DiscountedPrice { get; set; } = null!;
        public int Quantity { get; set; }
        public string Name { get; set; } = null!;
    }
}

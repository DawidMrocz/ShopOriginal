using Contracts.RabbitMessages;

namespace BasketApp.ApiModels
{
    public class BasketResponse
    {
        public int TotalQuantity => BasketItems.Count;
        public Price TotalPrice => new()
        {
            Value = BasketItems.Sum(p => p.DiscountedPrice.DiscountValue),
            Currency = BasketItems.Select(p => p.Price.Currency).First(),
        };

        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<BasketItemResponse> BasketItems { get; set; } = [];
        public List<DiscountResponse> Discounts { get; set; } = [];
    }

    public class DiscountResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Percentage { get; set; }
    }
}

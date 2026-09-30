namespace Contracts.RabbitMessages.Basket
{
    public class BasketItemUpdatedQuantity
    {
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}

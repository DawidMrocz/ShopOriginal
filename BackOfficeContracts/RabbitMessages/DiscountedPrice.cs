namespace BackOfficeContracts.RabbitMessages
{
    public class DiscountedPrice : Price
    {
        public int? DiscountId { get; set; }
        public decimal DiscountValue => DiscountPercentage is not null ? Value * (1 - (decimal)DiscountPercentage) : Value;
        public decimal? DiscountPercentage { get; set; }
    }
}

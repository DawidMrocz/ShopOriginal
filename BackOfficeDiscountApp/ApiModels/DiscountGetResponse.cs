namespace BackOfficeDiscountApp.ApiModels
{
    public class DiscountGetResponse
    {
        public int Id { get; set; }
        public decimal Percentage { get; set; }
        public string? Code { get; set; }
        public List<DiscountProductResponse> Products { get; set; } = new();
    }

    public class DiscountProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

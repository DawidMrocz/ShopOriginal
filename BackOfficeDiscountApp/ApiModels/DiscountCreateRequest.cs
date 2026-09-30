namespace BackOfficeDiscountApp.ApiModels
{
    public class DiscountCreateRequest
    {
        public decimal Percentage { get; set; }
        public string? Code { get; set; }
        public List<int> ProductIds { get; set; } = new();
    }
}

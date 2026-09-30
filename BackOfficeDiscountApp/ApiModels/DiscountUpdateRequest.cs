namespace BackOfficeDiscountApp.ApiModels
{
    public class DiscountUpdateRequest
    {
        public decimal Percentage { get; set; }
        public string? Code { get; set; }
        public int ProductId { get; set; }
    }
}

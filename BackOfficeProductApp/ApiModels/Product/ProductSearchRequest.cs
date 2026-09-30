namespace BackOfficeProductApp.ApiModels.Product
{
    public class ProductSearchRequest
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? SortOrder { get; set; }
        public string? SortBy { get; set; }
        public string? Name { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}

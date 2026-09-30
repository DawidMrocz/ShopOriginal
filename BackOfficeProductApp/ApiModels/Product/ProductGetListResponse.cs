namespace BackOfficeProductApp.ApiModels.Product
{
    public class ProductGetListResponse
    {
        public List<ProductGetResponse> Products { get; set; } = new();
    }
}

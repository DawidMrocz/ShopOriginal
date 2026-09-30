using BackOfficeProductApp.ApiModels.Product;

namespace BackOfficeProductApp.Services.ProductService
{
    public interface IProductService
    {
        Task<ProductGetResponse> Get(int productId, Guid? userId, string culture);
        Task<List<ProductGetResponse>> Search(ApiModels.Product.ProductSearchRequest request, string culture);
        Task Create(ProductCreateRequest request);
        Task Update(ProductUpdateRequest request);
        Task Delete(int productId);
        Task<object> Statistics();
    }
}

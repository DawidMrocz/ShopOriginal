using ProductApp.ApiModels.Product;

namespace ProductApp.Services.ProductService
{
    public interface IProductService
    {
        Task<ProductGetResponse> Get(int productId, Guid? userId, string culture);
        Task<List<ProductGetListResponse>> Search(ProductSearchRequest request, Guid? userId,string culture);
        Task AddToFavorites(int productId, Guid userId,string culture);
        Task DeleteToFavorites(int productId, Guid userId);
        Task AddToBasket(int productId, int quantity, Guid userId, string culture);
    }
}

using ProductApp.ApiModels.Category;

namespace ProductApp.Services.Category
{
    public interface ICategoryService
    {
        Task<List<CategoryGetResponse>> GetList();
        Task<CategoryGetResponse> Get(int categoryId);
    }
}

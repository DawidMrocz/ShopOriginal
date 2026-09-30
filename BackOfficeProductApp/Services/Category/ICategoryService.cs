using BackOfficeProductApp.ApiModels.Category;

namespace BackOfficeProductApp.Services.Category
{
    public interface ICategoryService
    {
        Task<List<CategoryGetResponse>> GetList();
        Task<CategoryGetResponse> Get(int categoryId);
        Task Create(string name);
        Task Delete(int categoryId);
    }
}

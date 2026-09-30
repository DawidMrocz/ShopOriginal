using BackOfficeProductApp.ApiModels.Category;
using Framework.Attribiutes.Dependency;
using Google.Protobuf.WellKnownTypes;
using ProductAppGrpc;

namespace BackOfficeProductApp.Services.Category
{
    [DependencyInjection(typeof(ICategoryService))]
    public class CategoryService : ICategoryService
    {
        private readonly ProductAppGrpc.Category.CategoryClient _client;

        public CategoryService(ProductAppGrpc.Category.CategoryClient client)
        {
            _client = client;
        }

        public async Task Create(string name)
        {
            _client.Create(new ProductAppGrpc.CategoryCreateRequest()
            {
                Name = name,
            });
        }

        public async Task Delete(int categoryId)
        {
            _client.Delete(new CategoryDeleteRequest()
            {
                CategoryId = categoryId
            });
        }

        public async Task<CategoryGetResponse> Get(int categoryId)
        {
            CategoryGetReply category = _client.Get(new CategoryGetRequest()
            {
                CategoryId = categoryId
            });

            return new CategoryGetResponse()
            {
                Id = category.BaseRecords.Id,
                Name = category.Name,
            };
        }

        public async Task<List<CategoryGetResponse>> GetList()
        {
            CategoryGetListReply categorys = _client.GetList(new Empty());

            return categorys.Categories.Select(p => new CategoryGetResponse()
            {
                Id = p.BaseRecords.Id,
                Name = p.Name,
            }).ToList();
        }
    }
}






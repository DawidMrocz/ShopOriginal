using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductAppGrpc.Data;

namespace ProductAppGrpc.Services
{
    public class CategoryService : Category.CategoryBase
    {
        private readonly ILogger<CategoryService> _logger;

        private readonly ProductContext _productContext;

        public CategoryService(ILogger<CategoryService> logger, ProductContext productContext)
        {
            _logger = logger;
            _productContext = productContext;
        }

        public override async Task<CategoryCreateReply> Create(CategoryCreateRequest request, ServerCallContext context)
        {
            try
            {
                Models.CategoryModel model = new()
                {
                    Name = request.Name,
                };

                await _productContext.Categories.AddAsync(model);

                await _productContext.SaveChangesAsync();

                return new CategoryCreateReply { CategoryId = model.Id };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<Empty> Delete(CategoryDeleteRequest request, ServerCallContext context)
        {
            try
            {
                Models.CategoryModel model = await _productContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId)
                        ?? throw new Exception("Category not found");

                _productContext.Categories.Remove(model);
                await _productContext.SaveChangesAsync();

                return new Empty();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<CategoryGetReply> Get(CategoryGetRequest request, ServerCallContext context)
        {
            try
            {
                Models.CategoryModel category = await _productContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CategoryId)
                        ?? throw new Exception("Category not found");

                return new CategoryGetReply()
                {
                    BaseRecords = new CategoryBaseRecord()
                    {
                        Id = category.Id,
                        Created = Timestamp.FromDateTime(category.Created.ToUniversalTime()),
                        CreatedById = category.CreatedById,
                        Updated = category.Updated is null ? null : Timestamp.FromDateTime(((DateTime)category.Updated).ToUniversalTime()),
                        UpdatedById = category.UpdatedById,
                    },
                    Name = category.Name,
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<CategoryGetListReply> GetList(Empty request, ServerCallContext context)
        {
            try
            {
                List<Models.CategoryModel> categories = await _productContext.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

                CategoryGetListReply categoriesReponse = new();

                categoriesReponse.Categories.AddRange(categories.Select(c => new CategoryGetReply()
                {
                    BaseRecords = new CategoryBaseRecord()
                    {
                        Id = c.Id,
                        Created = Timestamp.FromDateTime(c.Created.ToUniversalTime()),
                        CreatedById = c.CreatedById,
                        Updated = c.Updated is null ? null : Timestamp.FromDateTime(((DateTime)c.Updated).ToUniversalTime()),
                        UpdatedById = c.UpdatedById,
                    },
                    Name = c.Name,
                }));

                return categoriesReponse;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }
    }
}

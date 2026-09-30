using Framework.Models.File;
using Framework.Repositories.File;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductAppGrpc.Data;

namespace ProductAppGrpc.Services
{
    public class ProductService : Product.ProductBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IFileRepository _fileRepository;

        private readonly ProductContext _productContext;

        public ProductService(ILogger<ProductService> logger, ProductContext productContext, IFileRepository fileRepository)
        {
            _logger = logger;
            _productContext = productContext;
            _fileRepository = fileRepository;
        }

        public override async Task<ProductCreateReply> Create(ProductCreateRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProductModel model = new()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Quantity = request.Quantity,
                    Price = request.Price,
                    ProducerId = request.ProducerId,
                };

                await _productContext.Products.AddAsync(model);

                await _productContext.SaveChangesAsync();

                foreach (int categoryId in request.CategoryId)
                {
                    Models.CategoryModel category = await _productContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId)
                        ?? throw new Exception("Category do not exists");
                    model.Categories.Add(category);
                }

                if (request.Photo is not null)
                {
                    FileModel fileModel = await _fileRepository.CreateRaw(request.Photo.ToByteArray(), request.PhotoName);
                    model.Photo = fileModel.Id;
                }

                await _productContext.SaveChangesAsync();

                return new ProductCreateReply { ProductId = model.Id };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<Empty> Update(ProductUpdateRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProductModel model = await _productContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId)
               ?? throw new Exception("Product not found");

                model.Updated = DateTime.UtcNow;
                model.Name = request.Request.Name;
                model.Description = request.Request.Description;
                model.Price = request.Request.Price;
                model.Quantity = request.Request.Quantity;
                model.ProducerId = request.Request.ProducerId;

                if (request.Request.Photo is not null)
                {
                    await _fileRepository.Delete(model.Photo);
                    FileModel fileModel = await _fileRepository.CreateRaw(request.Request.Photo.ToByteArray(), request.Request.PhotoName);
                    model.Photo = fileModel.Id;
                }

                await _productContext.SaveChangesAsync();
                return new Empty();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<Empty> ChangeQuantity(ProductChangeQuantityRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProductModel model = await _productContext.Products.FirstOrDefaultAsync(c => c.Id == request.ProductId)
                        ?? throw new Exception("Product not found");

                if (model.Quantity < request.Quantity)
                    throw new Exception("Not enough products");

                model.Quantity =-- request.Quantity;

                _productContext.Products.Remove(model);
                await _productContext.SaveChangesAsync();

                return new Empty();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<Empty> Delete(ProductDeleteRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProductModel model = await _productContext.Products.FirstOrDefaultAsync(c => c.Id == request.ProductId)
                        ?? throw new Exception("Product not found");

                _productContext.Products.Remove(model);
                await _productContext.SaveChangesAsync();

                return new Empty();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<ProductGetReply> Get(ProductGetRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProductModel product = await _productContext.Products
                    .Include(p => p.Producer)
                    .Include(p => p.Categories)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == request.ProductId)
                        ?? throw new Exception("Product not found");

                FileModel file = await _fileRepository.Get(product.Photo);

                FileModel producerFile = await _fileRepository.Get(product.Producer.LogoId);

                ProductGetReply response = new()
                {
                    BaseRecords = new ProductBaseRecord()
                    {
                        Id = product.Id,
                        Created = Timestamp.FromDateTime(product.Created.ToUniversalTime()),
                        CreatedById = product.CreatedById,
                        Updated = product.Updated is null ? null : Timestamp.FromDateTime(((DateTime)product.Updated).ToUniversalTime()),
                        UpdatedById = product.UpdatedById,
                    },
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Description = product.Description,
                    IsAvailable = product.IsAvailable,
                    Price = decimal.ToInt64(product.Price * (decimal)request.CurrencyRate),
                    Photo = ByteString.CopyFrom(file.Content),
                    PhotoName = file.Name,
                    Producer = new ProductProducer()
                    {
                        BaseRecords = new ProductBaseRecord()
                        {
                            Id = product.Producer.Id,
                            Created = Timestamp.FromDateTime(product.Created.ToUniversalTime()),
                            CreatedById = product.Producer.CreatedById,
                            Updated = product.Producer.Updated is null ? null : Timestamp.FromDateTime(((DateTime)product.Updated).ToUniversalTime()),
                            UpdatedById = product.Producer.UpdatedById,
                        },
                        Name = product.Producer.Name,
                        Logo = ByteString.CopyFrom(producerFile.Content),
                        LogoName = producerFile.Name,
                    },
                };

                foreach (Models.CategoryModel? category in product.Categories)
                    response.Categories.Add(new ProductCategory()
                    {
                        BaseRecords = new ProductBaseRecord()
                        {
                            Id = product.Producer.Id,
                            Created = Timestamp.FromDateTime(product.Created.ToUniversalTime()),
                            CreatedById = product.Producer.CreatedById,
                            Updated = product.Producer.Updated is null ? null : Timestamp.FromDateTime(((DateTime)product.Updated).ToUniversalTime()),
                            UpdatedById = product.Producer.UpdatedById,
                        },
                        Name = category.Name,
                    });

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<ProductGetListReply> Search(ProductSearchRequest request, ServerCallContext context)
        {
            try
            {
                IQueryable<Models.ProductModel> results = _productContext.Products
                .Include(c => c.Categories)
                .Include(p => p.Producer)
                .AsNoTracking()
                .AsQueryable();

                if (request.Name is not null)
                    results = results.Where(r => r.Name.Contains(request.Name)).AsQueryable();

                if (request.PriceFrom is not null)
                    results = results.Where(r => r.Price * (decimal)request.CurrencyRate >= (decimal)request.PriceFrom).AsQueryable();

                if (request.PriceTo is not null)
                    results = results.Where(r => r.Price * (decimal)request.CurrencyRate <= (decimal)request.PriceTo).AsQueryable();

                request.SortBy ??= "name";

                var sortedResponse = request.SortBy.ToLower() switch
                {
                    "name" when request.SortOrder >= 0 => results.OrderBy(o => o.Name),
                    "name" when request.SortOrder < 0 => results.OrderByDescending(o => o.Name),
                    "price" when request.SortOrder >= 0 => results.OrderBy(o => o.Price),
                    "price" when request.SortOrder < 0 => results.OrderByDescending(o => o.Price),
                    _ => results.OrderBy(o => o.Name),
                };

                request.Page ??= 1;
                request.PageSize ??= 20;

                results = sortedResponse.Skip(((int)request.Page - 1) * (int)request.PageSize).Take((int)request.PageSize).AsQueryable();

                ProductGetListReply producersReponse = new();

                foreach (Models.ProductModel c in results)
                {
                    ProductGetReply reply = new()
                    {
                        BaseRecords = new ProductBaseRecord()
                        {
                            Id = c.Id,
                            Created = Timestamp.FromDateTime(c.Created.ToUniversalTime()),
                            CreatedById = c.CreatedById,
                            Updated = c.Updated == null ? null : Timestamp.FromDateTime(((DateTime)c.Updated).ToUniversalTime()),
                            UpdatedById = c.UpdatedById,
                        },
                        Name = c.Name,
                        Price = decimal.ToInt64(c.Price * (decimal)request.CurrencyRate),
                        IsAvailable = c.IsAvailable,
                        Description = c.Description,
                        Quantity = c.Quantity,
                        Producer = new ProductProducer()
                        {
                            BaseRecords = new ProductBaseRecord()
                            {
                                Id = c.Producer.Id,
                                Created = Timestamp.FromDateTime(c.Created.ToUniversalTime()),
                                CreatedById = c.Producer.CreatedById,
                                Updated = c.Producer.Updated == null ? null : Timestamp.FromDateTime(((DateTime)c.Updated).ToUniversalTime()),
                                UpdatedById = c.Producer.UpdatedById,
                            },
                        },
                    };

                    foreach (Models.CategoryModel cat in c.Categories)
                    {
                        reply.Categories.Add(new ProductCategory()
                        {
                            BaseRecords = new ProductBaseRecord()
                            {
                                Id = cat.Id,
                                Created = Timestamp.FromDateTime(cat.Created.ToUniversalTime()),
                                CreatedById = cat.CreatedById,
                                Updated = cat.Updated == null ? null : Timestamp.FromDateTime(((DateTime)c.Updated).ToUniversalTime()),
                                UpdatedById = cat.UpdatedById,
                            },
                            Name = c.Name,
                        });
                    }

                    producersReponse.Products.Add(reply);
                }
            
                return producersReponse;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }
    }
}

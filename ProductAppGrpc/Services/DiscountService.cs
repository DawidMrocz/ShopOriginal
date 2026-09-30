using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductAppGrpc.Data;

namespace ProductAppGrpc.Services
{
    public class DiscountService : Discount.DiscountBase
    {
        private readonly ILogger<DiscountService> _logger;

        private readonly ProductContext _productContext;

        public DiscountService(ILogger<DiscountService> logger, ProductContext productContext)
        {
            _logger = logger;
            _productContext = productContext;
        }

        public override async Task<DiscountCreateReply> Create(DiscountCreateRequest request, ServerCallContext context)
        {
            try
            {
                Models.DiscountModel model = new()
                {
                    Code = request.Code,
                    Percentage = request.Percentage,
                    ValidFrom = request.ValidFrom.ToDateTime(),
                    ValidTo = request.ValidTo.ToDateTime(),
                };

                await _productContext.Discounts.AddAsync(model);

                await _productContext.SaveChangesAsync();

                foreach (DiscountCreateRequestProductIds product in request.Products)
                {
                    model.Products.Add(new Models.ProductModel()
                    {
                        Id = product.ProductId,
                    });
                }

                return new DiscountCreateReply { DiscountId = model.Id };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<Empty> Delete(DiscountDeleteRequest request, ServerCallContext context)
        {
            try
            {
                Models.DiscountModel model = await _productContext.Discounts.FirstOrDefaultAsync(c => c.Id == request.DiscountId)
                        ?? throw new Exception("Discount not found");

                _productContext.Discounts.Remove(model);
                await _productContext.SaveChangesAsync();

                return new Empty();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<DiscountGetReply> Get(DiscountGetRequest request, ServerCallContext context)
        {
            try
            {
                Models.DiscountModel discount = await _productContext.Discounts.Include(p => p.Products).AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.DiscountId)
                        ?? throw new Exception("Discount not found");

                DiscountGetReply discountResponse =  new()
                {
                    BaseRecords = new DiscountBaseRecord()
                    {
                        Id = discount.Id,
                        Created = Timestamp.FromDateTime(discount.Created.ToUniversalTime()),
                        CreatedById = discount.CreatedById,
                        Updated = discount.Updated is null ? null : Timestamp.FromDateTime(((DateTime)discount.Updated).ToUniversalTime()),
                        UpdatedById = discount.UpdatedById,
                    },
                    Code = discount.Code,
                    Percentage = decimal.ToInt64(discount.Percentage),
                };

                foreach (Models.ProductModel product in discount.Products) 
                {
                    discountResponse.Products.Add(new DiscountGetProductReply()
                    {
                        BaseRecords = new DiscountBaseRecord()
                        {
                            Id = product.Id,
                            Created = Timestamp.FromDateTime(product.Created.ToUniversalTime()),
                            CreatedById = product.CreatedById,
                            Updated = product.Updated is null ? null : Timestamp.FromDateTime(((DateTime)product.Updated).ToUniversalTime()),
                            UpdatedById = product.UpdatedById,
                        },
                        ProductId = product.Id,
                        ProductName = product.Name,
                    });
                }

                return discountResponse;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<DiscountGetListReply> GetList(Empty request, ServerCallContext context)
        {
            try
            {
                List<Models.DiscountModel> discounts = await _productContext.Discounts.Include(p => p.Products).AsNoTracking().ToListAsync();

                DiscountGetListReply discountsReponse = new();

                foreach (Models.DiscountModel discount in discounts)
                {
                    var newModel = new DiscountGetReply()
                    {
                        BaseRecords = new DiscountBaseRecord()
                        {
                            Id = discount.Id,
                            Created = Timestamp.FromDateTime(discount.Created.ToUniversalTime()),
                            CreatedById = discount.CreatedById,
                            Updated = discount.Updated is null ? null : Timestamp.FromDateTime(((DateTime)discount.Updated).ToUniversalTime()),
                            UpdatedById = discount.UpdatedById,
                        },
                        Code = discount.Code,
                        Percentage = decimal.ToInt64(discount.Percentage),
                    };

                    foreach (Models.ProductModel product in discount.Products)
                    {
                        newModel.Products.Add(new DiscountGetProductReply()
                        {
                            BaseRecords = new DiscountBaseRecord()
                            {
                                Id = product.Id,
                                Created = Timestamp.FromDateTime(product.Created.ToUniversalTime()),
                                CreatedById = product.CreatedById,
                                Updated = product.Updated is null ? null : Timestamp.FromDateTime(((DateTime)product.Updated).ToUniversalTime()),
                                UpdatedById = product.UpdatedById,
                            },
                            ProductId = product.Id,
                            ProductName = product.Name,
                        });
                    }

                    discountsReponse.Discounts.Add(newModel);
                }
             
                return discountsReponse;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }
    }
}

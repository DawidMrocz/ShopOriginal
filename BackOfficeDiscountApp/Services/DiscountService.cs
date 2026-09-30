using BackOfficeDiscountApp.ApiModels;
using BackOfficeDiscountApp.Data;
using BackOfficeDiscountApp.Models;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using ProductAppGrpc;

namespace BackOfficeDiscountApp.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly DiscountContext _discountContext;
        private readonly ProductAppGrpc.Discount.DiscountClient _client;

        public DiscountService(DiscountContext discountContext, ProductAppGrpc.Discount.DiscountClient client)
        {
            _discountContext = discountContext;
            _client = client;
        }

        public async Task Create(ApiModels.DiscountCreateRequest request)
        {
            Models.Discount model = new()
            {
                Code = request.Code,
                Percentage = request.Percentage,
            };

            await _discountContext.Discounts.AddAsync(model);

            await _discountContext.SaveChangesAsync();

            model.Products.AddRange(request.ProductIds.Select(id => new Models.DiscoutProduct()
            {
                DiscountId = model.Id,
                ProductId = id,
            }));

            await _discountContext.SaveChangesAsync();

            var discount = new ProductAppGrpc.DiscountCreateRequest()
            {
                Code = model.Code,
                Percentage = decimal.ToInt64(model.Percentage),
                ValidFrom = Timestamp.FromDateTime(((DateTime)model.ValidFrom).ToUniversalTime()),
                ValidTo = Timestamp.FromDateTime(((DateTime)model.ValidTo).ToUniversalTime()),
            };

            RepeatedField<DiscountCreateRequestProductIds> products = new();

            foreach (Models.DiscoutProduct product in model.Products)
            {
                products.Add(new DiscountCreateRequestProductIds()
                {
                    ProductId = product.ProductId,
                });
            }

            discount.Products.AddRange(products);

            _client.Create(discount);
        }

        public async Task Delete(int discountId)
        {
            Models.Discount model = await _discountContext.Discounts.FirstOrDefaultAsync(d => d.Id == discountId)
                ?? throw new Exception("Discount not found");

            _discountContext.Discounts.Remove(model);
            await _discountContext.SaveChangesAsync();
        }

        public async Task<DiscountGetResponse> Get(int discountId, Guid? userId, string culture)
        {
            Models.Discount model = await _discountContext.Discounts.FirstOrDefaultAsync(d => d.Id == discountId)
                ?? throw new Exception("Discount not found");

            DiscountGetReply discount = _client.Get(new DiscountGetRequest()
            {
                DiscountId = discountId,
            });

            return new DiscountGetResponse()
            {
                Id = discount.BaseRecords.Id,
                Code = discount.Code,
                Percentage= discount.Percentage,
                Products = discount.Products.Select(p => new DiscountProductResponse()
                {
                    Id= p.ProductId,
                    Name = p.ProductName,
                }).ToList(),
            };
        }

        public Task<List<DiscountGetListResponse>> Search(DiscountSearchRequest request, Guid? userId, string culture)
        {
            throw new NotImplementedException();
        }
    }
}

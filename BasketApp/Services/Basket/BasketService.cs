using BasketApp.ApiModels;
using BasketApp.Data;
using BasketApp.Models;
using Contracts.RabbitMessages;
using Framework.Models.Currency;
using Framework.Repositories.Currency;
using Microsoft.EntityFrameworkCore;


namespace BasketApp.Services.Basket
{
    public class BasketService : IBasketService
    {
        private readonly BasketContext _basketContext;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ProductAppGrpc.Product.ProductClient _clientProduct;
        private readonly ProductAppGrpc.Discount.DiscountClient _clientDiscount;

        public BasketService(BasketContext basketContext, ICurrencyRepository currencyRepository)
        {
            _basketContext = basketContext;
            _currencyRepository = currencyRepository;
        }

        public async Task<BasketResponse> Get(Guid userId, string culture)
        {
            User user = await _basketContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId)
               ?? throw new Exception("User not found");

            Models.Basket response = await _basketContext.Baskets
                .Include(b => b.User)
                .ThenInclude(d => d.Discounts.Where(d => d.Used == false))
                .Include(b => b.BasketItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == user.Id)
            ?? throw new Exception("Basket not found");

            CurrencyModel currency = await _currencyRepository.Get(culture == "pl-Pl" ? 1 : 2);

            ProductAppGrpc.ProductGetListReply products = _clientProduct.Search(new ProductAppGrpc.ProductSearchRequest()
            {
                Page = 1,
                PageSize = int.MaxValue,
            });

            ProductAppGrpc.DiscountGetListReply discounts = _clientDiscount.GetList(new Google.Protobuf.WellKnownTypes.Empty());

            List<ProductAppGrpc.DiscountGetReply> userDiscounts = discounts.Discounts.Where(d => response.User.Discounts.Select(d => d.Id).ToList().Contains(d.BaseRecords.Id)).ToList();

            BasketResponse result = new()
            {
                Email = response.User.Email,
                FirstName = response.User.FirstName,
                LastName = response.User.LastName,
                BasketItems = response.BasketItems.Select(b => new BasketItemResponse()
                {
                    ProductId = b.ProductId,
                    Price = new Price()
                    {
                        Value = b.Price * currency.Rate,
                        Currency = currency.Name,
                    },
                    Quantity = b.Quantity,
                    Name = products.Products.Single(p => p.BaseRecords.Id == b.ProductId).Name,
                }).ToList()
            };

            foreach (BasketItemResponse item in result.BasketItems)
            {
                List<ProductAppGrpc.DiscountGetReply> itemDiscounts = userDiscounts.Where(d => d.Products.Select(p => p.ProductId).Contains(item.ProductId)).ToList();

                if (!itemDiscounts.Any()) continue;

                ProductAppGrpc.DiscountGetReply? bestItemDiscount = itemDiscounts.MaxBy(d => d.Percentage);

                item.DiscountedPrice = new()
                {
                    DiscountId = bestItemDiscount?.BaseRecords.Id,
                    Currency = currency.Name,
                    DiscountPercentage = bestItemDiscount?.Percentage,
                    Value = item.Price.Value * currency.Rate
                };
            }
            return result;
        }
    }
}

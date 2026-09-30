using Contracts.RabbitMessages;
using Contracts.RabbitMessages.Favorite;
using Framework.Attribiutes.Dependency;
using Framework.Models.Currency;
using Framework.Repositories.Currency;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserApp.ApiModels.Favorite;
using UserApp.Data;
using UserApp.Models;

namespace UserApp.Services.Favorite
{
    [DependencyInjection(typeof(IFavoriteService))]
    public class FavoriteService : IFavoriteService
    {
        private readonly UserContext _userContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ProductAppGrpc.Product.ProductClient _productClient;
        public FavoriteService(UserContext userContext, IPublishEndpoint publishEndpoint, ICurrencyRepository currencyRepository, ProductAppGrpc.Product.ProductClient productClient)
        {
            _userContext = userContext;
            _publishEndpoint = publishEndpoint;
            _currencyRepository = currencyRepository;
            _productClient = productClient;
        }

        public async Task<int> Create(Guid userId)
        {
            Models.Favorite model = new() { UserId = userId, };

            await _userContext.Favorites.AddAsync(model);

            await _userContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new FavoriteCreated()
            {
                Id = model.Id,
                UserId = userId,
            });

            return model.Id;
        }

        public async Task Delete(int favoriteItemId, Guid userId)
        {
            Models.Favorite model = await _userContext.Favorites.FirstOrDefaultAsync(f => f.UserId == userId)
                ?? throw new Exception("Not found");

            FavoriteItem item = model.Items.FirstOrDefault(f => f.Id == favoriteItemId)
                ?? throw new Exception("Item not found");

            model.Items.Remove(item);

            await _userContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new FavoriteDeleted()
            {
                Id = item.Id,
            });
        }

        public async Task<FavoriteResponse> Get(Guid userId, string culture)
        {
            Models.Favorite models = await _userContext.Favorites
                .Include(u => u.User)
                .Include(p => p.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == userId)
            ?? throw new Exception("Favorite store not found");

            ProductAppGrpc.ProductGetListReply products = _productClient.Search(new ProductAppGrpc.ProductSearchRequest()
            {
                Page = 1,
                PageSize = int.MaxValue,
            });

            CurrencyModel currency = await _currencyRepository.GetByCode(culture == "pl-PL" ? "PLN" : "EUR");

            FavoriteResponse response = new()
            {
                Id = models.Id,
                FirstName = models.User.FirstName,
                LastName = models.User?.LastName,
            };

            foreach (FavoriteItem item in models.Items)
            {
                ProductAppGrpc.ProductGetReply product = products.Products.FirstOrDefault(p => p.BaseRecords.Id == item.ProductId)
                    ?? throw new Exception("Product do not exists");

                response.Items.Add(new FavoriteResponseItem()
                {
                    Id = item.Id,
                    Name = product.Name,
                    Price = new Price()
                    {
                        Value = product.Price * currency.Rate,
                        Currency = currency.Code,
                    },
                    ProducerName = product.Producer.Name,
                    ProducerLogo = product.Producer.Logo.ToByteArray(),
                });
            }

            return response;
        }
    }
}

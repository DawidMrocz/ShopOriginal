using Contracts.RabbitMessages;
using Contracts.RabbitMessages.Basket;
using Contracts.RabbitMessages.Favorite;
using Framework.Attribiutes.Dependency;
using Framework.Models.Currency;
using Framework.Repositories.Currency;
using Framework.Repositories.File;
using Grpc.Net.Client;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApp.ApiModels.Category;
using ProductApp.ApiModels.Producer;
using ProductApp.ApiModels.Product;
using ProductApp.Data;
using ProductApp.Models;
using ProductApp.Services.Category;
using ProductApp.Services.Producer;
using ProductAppGrpc;


namespace ProductApp.Services.ProductService
{
    [DependencyInjection(typeof(IProductService))]
    public partial class ProductService : IProductService
    {
        private readonly ProductContext _productContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ProductAppGrpc.Product.ProductClient _productClient;


        public ProductService( ProductContext catalogContext, IPublishEndpoint publishEndpoint, ICurrencyRepository currencyRepository, Product.ProductClient productClient)
        {
            _productContext = catalogContext;
            _publishEndpoint = publishEndpoint;
            _currencyRepository = currencyRepository;
            _productClient = productClient;
        }

        public async Task AddToFavorites(int productId, Guid userId, string culture)
        {
            Favorite model = await _productContext.Favorities.Include(f => f.User).FirstOrDefaultAsync(f => f.User.ExternalId == userId)
                ?? throw new Exception("Favorite not found");

            CurrencyModel currency = await _currencyRepository.GetByCode(culture == "pl-PL" ? "PLN" : "EUR");

            ProductGetReply product = _productClient.Get(new ProductAppGrpc.ProductGetRequest()
            {
                ProductId = productId,
                CurrencyRate = (double)currency.Rate,
            });

            FavoriteItem item = new()
            {
                FavoriteId = model.Id,
                ProductId = product.BaseRecords.Id,
            };

            await _productContext.FavoriteItems.AddAsync(item);
            await _productContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new FavoriteItemCreated()
            {
                Id = item.Id,
                FavoriteId = model.ExternalId,
                ProductId = productId
            });
        }

        public async Task DeleteToFavorites(int productId, Guid userId)
        {
            Favorite model = await _productContext.Favorities.Include(f => f.User).FirstOrDefaultAsync(f => f.User.ExternalId == userId)
                ?? throw new Exception("Favorite not found");

            FavoriteItem? item = model.Items.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new Exception("Favcrite item not found");

            model.Items.Remove(item);

            await _productContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new FavoriteDeleted()
            {
                Id = model.Id,
            });
        }

        public async Task<ProductGetResponse> Get(int productId, Guid? userId, string culture)
        {
            CurrencyModel currency = await _currencyRepository.GetByCode(culture == "pl-PL" ? "PLN" : "EUR");

            ProductGetReply product = _productClient.Get(new ProductAppGrpc.ProductGetRequest()
            {
                ProductId = productId,
                CurrencyRate = (double)currency.Rate,
            });

            List<int> favoriteProducts = [];

            if (userId is not null)
            {
                User user = await _productContext.Users.Include(u => u.Favorites).ThenInclude(i => i.Items).FirstOrDefaultAsync(u => u.ExternalId == userId)
                    ?? throw new Exception("User not found");

                favoriteProducts.AddRange(user.Favorites.Items.Select(f => f.ProductId).ToList());
            }

            List<Models.Comment> comments = await _productContext.Comments.Where(p => p.ProductId == product.BaseRecords.Id).ToListAsync();

            return new ProductGetResponse()
            {
                Id = productId,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Price = new Price() { Value = product.Price * currency.Rate, Currency = currency.Name },
                IsFavorite = favoriteProducts.Contains(product.BaseRecords.Id),
                Producer = new ProducerGetResponse()
                {
                    Id = product.Producer.BaseRecords.Id,
                    Name = product.Producer.Name,
                    Logo = product.Producer.Logo.ToByteArray()
                },
                Categories = product.Categories.Select(c => new CategoryGetResponse()
                {
                    Id = c.BaseRecords.Id,
                    Name = c.Name,
                }).ToList(),
                Photo = product.Photo.ToByteArray(),
                Comments = comments.Select(c => new ApiModels.Comment.CommentGetResponse()
                {
                    Id = c.Id,
                    Created = c.Created,
                    Updated = c.Updated,
                    Title = c.Title,
                    Description = c.Description,
                    Rate = c.Rate,
                }).ToList()
            };
        }

        public async Task AddToBasket(int productId, int quantity, Guid userId, string culture)
        {
            User user = await _productContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId)
               ?? throw new Exception("User not found");

            CurrencyModel currency = await _currencyRepository.GetByCode(culture == "pl-PL" ? "PLN" : "EUR");

            ProductGetReply product = _productClient.Get(new ProductAppGrpc.ProductGetRequest()
            {
                ProductId = productId,
                CurrencyRate = (double)currency.Rate,
            });

            if (quantity <= 0) throw new Exception("Quantity cannot be equal or less 0");

            if (product.Quantity < quantity) throw new Exception("NIe ma tyle sztuk");

            product.Quantity = --quantity;

            await _productContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new BasketItemCreated()
            {
                ProductId = product.BaseRecords.Id,
                UserId = userId,
                Quantity = quantity,
                ProductQuantity = product.Quantity,
            });
        }

        public async Task<List<ProductGetListResponse>> Search(ApiModels.Product.ProductSearchRequest request, Guid? userId, string culture)
        {
            CurrencyModel currency = await _currencyRepository.GetByCode(culture == "pl-PL" ? "PLN" : "EUR");

            ProductGetListReply results = _productClient.Search(new ProductAppGrpc.ProductSearchRequest()
            {
                Page = request.Page,
                PageSize = request.PageSize,
                PriceFrom = request.PriceFrom is null ? null : decimal.ToInt64((decimal)request.PriceFrom * currency.Rate),
                PriceTo = request.PriceTo is null ? null : decimal.ToInt64((decimal)request.PriceTo * currency.Rate),
                CurrencyRate = (double)currency.Rate,
                Name = request.Name,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder,
            });

            User user;
            List<int> favoriteProducts = [];

            if (userId is not null)
            {
                user = await _productContext.Users.Include(u => u.Favorites).ThenInclude(i => i.Items).FirstOrDefaultAsync(u => u.ExternalId == userId)
                    ?? throw new Exception("User not found");

                favoriteProducts.AddRange(user.Favorites.Items.Select(f => f.ProductId).ToList());
            }

            List<Models.Comment> allComments = await _productContext.Comments.ToListAsync();

            return results.Products.Select(r => new ProductGetListResponse()
            {
                Id = r.BaseRecords.Id,
                Name = r.Name,
                Price = new Price()
                {
                    Value = r.Price * currency.Rate,
                    Currency = currency.Code,
                },
                IsFavourite = favoriteProducts.Contains(r.BaseRecords.Id),
                IsAvailable = r.IsAvailable,
                CountOfComments = allComments.Where(c => c.ProductId == r.BaseRecords.Id).Count(),
                OverallRate = allComments.Where(c => c.ProductId == r.BaseRecords.Id && c.Rate is not null).Sum(c => c.Rate) / allComments.Where(c => c.ProductId == r.BaseRecords.Id && c.Rate is not null).Count(),
            }).ToList();
        }
    }
}

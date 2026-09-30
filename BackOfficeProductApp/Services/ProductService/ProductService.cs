using BackOfficeProductApp.ApiModels.Category;
using BackOfficeProductApp.ApiModels.Producer;
using BackOfficeProductApp.ApiModels.Product;
using BackOfficeContracts.RabbitMessages;
using Framework.Attribiutes.Dependency;
using Framework.Models.Currency;
using Framework.Models.File;
using Framework.Repositories.Currency;
using Framework.Repositories.File;
using Google.Protobuf;
using ProductAppGrpc;


namespace BackOfficeProductApp.Services.ProductService
{
    [DependencyInjection(typeof(IProductService))]
    public partial class ProductService : IProductService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ProductAppGrpc.Product.ProductClient _productClient;

        public ProductService(IFileRepository fileRepository, ICurrencyRepository currencyRepository, ProductAppGrpc.Product.ProductClient productClient)
        {
            _fileRepository = fileRepository;
            _currencyRepository = currencyRepository;
            _productClient = productClient;
        }

        public async Task Create(ApiModels.Product.ProductCreateRequest request)
        {
            ProductAppGrpc.ProductCreateRequest newProduct = new()
            {
                Description = request.Description,
                Name = request.Name,
                Price = decimal.ToInt64((decimal)request.Price),
                ProducerId = request.ProducerId,
                Quantity = request.Quantity,
            };

            foreach (int categoryId in request.CategoriesId)
                newProduct.CategoryId.Add(categoryId);

            byte[]? content = _fileRepository.GetContent(request.Photo);

            newProduct.Photo = ByteString.CopyFrom(content);
            newProduct.PhotoName = request.Photo.Name;

            _productClient.Create(newProduct);
        }

        public async Task Delete(int productId)
        {
            _productClient.Delete(new ProductAppGrpc.ProductDeleteRequest()
            {
                ProductId = productId,
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

            ProductGetResponse response = new()
            {
                Id = productId,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Price = new Price() { Value = product.Price * currency.Rate, Currency = currency.Name },
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
            };
            return response;
        }

        public async Task<List<ProductGetResponse>> Search(ApiModels.Product.ProductSearchRequest request, string culture)
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

            return results.Products.Select(r => new ProductGetResponse()
            {
                Id = r.BaseRecords.Id,
                Name = r.Name,
                Price = new Price()
                {
                    Value = r.Price * currency.Rate,
                    Currency = currency.Code,
                },
                Producer = new ProducerGetResponse()
                {
                    Id = r.Producer.BaseRecords.Id,
                    Name = r.Producer.Name,
                    Logo = r.Producer.Logo.ToByteArray()
                },
                Categories = r.Categories.Select(c => new CategoryGetResponse()
                {
                    Id = c.BaseRecords.Id,
                    Name = c.Name,
                }).ToList(),
                Photo = r.Photo.ToByteArray(),
            }).ToList();
        }

        public async Task<object> Statistics()
        {
            return new object();
        }

        public async Task Update(ApiModels.Product.ProductUpdateRequest request)
        {
            ProductAppGrpc.ProductUpdateRequest newProduct = new()
            {
                ProductId = request.Id,
                Request = new ProductAppGrpc.ProductCreateRequest()
                {
                    Description = request.Description,
                    Name = request.Name,
                    Price = decimal.ToInt64((decimal)request.Price),
                    ProducerId = request.ProducerId,
                    Quantity = request.Quantity,
                }
            };

            foreach (int categoryId in request.CategoryIds)
                newProduct.Request.CategoryId.Add(categoryId);

            FileModel file = await _fileRepository.Create(request.Photo);

            newProduct.Request.Photo = ByteString.CopyFrom(file.Content);
            newProduct.Request.PhotoName = file.Name;

            _productClient.Update(newProduct);
        }
    }
}

using BasketApp.ApiModels;

namespace BasketApp.Services.Basket
{
    public interface IBasketService
    {
        Task<BasketResponse> Get(Guid userId, string culture);
    }
}

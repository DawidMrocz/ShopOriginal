using BackOfficeDiscountApp.ApiModels;

namespace BackOfficeDiscountApp.Services
{
    public interface IDiscountService
    {
        Task<DiscountGetResponse> Get(int discountId, Guid? userId, string culture);
        Task<List<DiscountGetListResponse>> Search(DiscountSearchRequest request, Guid? userId, string culture);
        Task Create(DiscountCreateRequest request);
        Task Delete(int discountId);   
    }
}

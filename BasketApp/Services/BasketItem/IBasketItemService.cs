namespace BasketApp.Services.BasketItem
{
    public interface IBasketItemService
    {       
        Task Delete(int basketItemId, Guid userId);
        Task UpdateQuantity(int basketItemId, int quantity, Guid userId);
    }
}

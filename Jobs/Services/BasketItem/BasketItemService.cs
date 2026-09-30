using Framework.Attribiutes.Dependency;

namespace Jobs.Services.BasketItem
{
    [DependencyInjection(typeof(IBasketItemService))]
    internal class BasketItemService : IBasketItemService
    {
        public async Task DeleteOldItems()
        {
            Console.WriteLine("Usuwam stare basket itemy");
        }
    }
}

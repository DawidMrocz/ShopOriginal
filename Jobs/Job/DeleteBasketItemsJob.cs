using Jobs.Services.BasketItem;
using Microsoft.Extensions.DependencyInjection;

namespace Jobs.Job
{
    public class DeleteBasketItemsJob : BaseJob
    {
        private readonly IBasketItemService _basketItemService;

        public DeleteBasketItemsJob(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _basketItemService = serviceProvider.GetService<IBasketItemService>()!;
        }

        public override async Task JobRun() => await _basketItemService.DeleteOldItems();
    }
}

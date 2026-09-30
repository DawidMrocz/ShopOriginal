using BasketApp.Services.BasketItem;
using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BasketApp.Controllers
{
    [Route("[controller]")]
    public partial class BasketItemController : AuthorizedController
    {
        private readonly IBasketItemService _basketItemService;
        public BasketItemController(IServiceProvider serviceProvider, IBasketItemService basketItemService) : base(serviceProvider)
        {
            _basketItemService = basketItemService;
        }
    }
}

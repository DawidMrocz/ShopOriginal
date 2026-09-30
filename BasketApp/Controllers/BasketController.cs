using BasketApp.Services.Basket;
using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BasketApp.Controllers
{
    [Route("[controller]")]
    public partial class BasketController : AuthorizedController
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _basketService = basketService;
        }
    }
}

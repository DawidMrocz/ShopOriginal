using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using ProductApp.ApiModels.Producer;
using ProductApp.Services.Producer;

namespace ProductApp.Controllers
{
    [Route("[controller]")]
    public partial class ProducerController : AuthorizedController
    {
        private readonly IProducerService _producerService;

        public ProducerController(IServiceProvider serviceProvider, IProducerService producerService) : base(serviceProvider)
        {
            _producerService = producerService;
        }

        [HttpGet("producers")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _producerService.GetList());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

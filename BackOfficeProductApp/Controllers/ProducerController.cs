using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using BackOfficeProductApp.ApiModels.Producer;
using BackOfficeProductApp.Services.Producer;

namespace BackOfficeProductApp.Controllers
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

        [HttpPost("producers")]
        public async Task<IActionResult> Create([FromForm] ProducerCreateRequest request)
        {
            try
            {
                await _producerService.Create(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("producers/{producerId}")]
        public async Task<IActionResult> Delete([FromRoute] int producerId)
        {
            try
            {
                await _producerService.Delete(producerId);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

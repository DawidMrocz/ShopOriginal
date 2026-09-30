using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using UserApp.Repositories;

namespace UserApp.Controllers
{
    [Route("[controller]")]
    public class FavoriteController : AuthorizedController
    {
        private readonly IUnitOfWork _unitOfWork;
        public FavoriteController(IServiceProvider serviceProvider, IUnitOfWork unitOfWork) : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _unitOfWork.FavoriteService.Get(Guid.Parse(AuthorizedUser.Id), Culture));
        }

        [HttpDelete("/{favoriteId}")]
        public async Task<IActionResult> Delete([FromRoute] int favoriteId)
        {
            await _unitOfWork.FavoriteService.Delete(favoriteId, Guid.Parse(AuthorizedUser.Id));
            return Ok();
        }
    }
}

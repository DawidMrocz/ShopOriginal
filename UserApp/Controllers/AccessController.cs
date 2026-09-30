using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UserApp.Controllers
{
    [Route("[controller]")]
    public class AccessController(IServiceProvider serviceProvider) : AccessBaseController(serviceProvider)
    {
    }
}

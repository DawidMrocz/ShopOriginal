using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeUserApp.Controllers
{
    [Route("[controller]")]
    public class AccessController(IServiceProvider serviceProvider) : AccessBaseController(serviceProvider)
    {
    }
}

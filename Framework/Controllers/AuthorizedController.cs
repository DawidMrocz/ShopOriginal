using Framework.Configuration;
using Framework.Middlewares;
using Framework.Models.Authentication;
using Framework.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Controllers
{
    //[Autoryzacja]
    public abstract class AuthorizedController(IServiceProvider serviceProvider) : ControllerBase
    {
        protected readonly IServiceProvider _serviceProvider = serviceProvider;
        public string Culture => (HttpContext.Request.Cookies.TryGetValue("culture", out string? value) ? value : "pl-PL") ?? "pl-PL";
        protected AuthenticationUser? AuthorizedUser => AuthenticationService.GetAuthenticatedUser().Result;
        protected IAuthenticationService AuthenticationService => _serviceProvider
                .GetRequiredKeyedService<IAuthenticationService>(FrameworkConfiguration.AuthorizationProvider)
                    ?? throw new Exception("Authorization provider not found");
    }
}

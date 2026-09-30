using Framework.ApiModels.Access.Request;
using Framework.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Framework.Controllers
{
    public abstract class AccessBaseController : AuthorizedController
    {
        private readonly IConfiguration _configuration;
        protected AccessBaseController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration ?? throw new Exception("Service not found"); ;
        }

        [HttpPost("logout/external")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogout(string clientName, string? returnUrl = null)
        {
            try
            {
                AuthenticationService.Logout();

                List<SSOClientDto> ssoClients = _configuration.GetSection("SSO").Get<List<SSOClientDto>>()
                ?? throw new Exception("Appsetting not found");

                SSOClientDto client = ssoClients.FirstOrDefault(c => c.ClientName == clientName)
                    ?? throw new Exception("Client not found");

                string logoutAddress = client.Authority
                    ?? throw new Exception("Client logout address not found");

                if (!logoutAddress.EndsWith("/"))
                    logoutAddress = logoutAddress + "/";

                logoutAddress = logoutAddress + "connect/logout?returnUrl=" + (string.IsNullOrEmpty(returnUrl) ? client.RedirectAfterLogon : returnUrl);

                if (ssoClients.Any())
                    return new RedirectResult(logoutAddress);
                //TODO Prawidłowe zachowanie dla OIDC return SignOut(new AuthenticationProperties { RedirectUri = string.IsNullOrEmpty(returnUrl) ? Configuration.SsoRedirectAfterLogonUrl : returnUrl}, CookieAuthenticationDefaults.AuthenticationScheme, Configuration.SsoScheme);
                return Ok("Wylogowano");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("login/external")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin(string clientName, string? returnUrl = null)
        {
            List<SSOClientDto> ssoClients = _configuration.GetSection("SSO").Get<List<SSOClientDto>>()
               ?? throw new Exception("Appsetting not found");

            SSOClientDto client = ssoClients.FirstOrDefault(c => c.ClientName == clientName)
                ?? throw new Exception("Client not found");

            string logoutAddress = client.Authority
                ?? throw new Exception("Client logout address not found");

            if (AuthorizedUser is null)
                return Challenge(new AuthenticationProperties { RedirectUri = string.IsNullOrEmpty(returnUrl) ? client.RedirectAfterLogon : returnUrl }, "scheme" /*Configuration.SsoScheme*/);

            if (string.IsNullOrEmpty(returnUrl))
            {
                if (!string.IsNullOrWhiteSpace(client.RedirectAfterLogon))
                    return Redirect(client.RedirectAfterLogon);
                return Ok();
            }
            return Redirect(returnUrl);
        }

        [HttpPost("login/otp-verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(Guid guid, string passCode)
        {
            try
            {
                await AuthenticationService.VerifyOneTimePasscode(guid, passCode);
                return Ok("User has been logged in");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string? ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
                string? userAgent = HttpContext?.Request.Headers["User-Agent"].ToString();

                bool fromConfiguration = true;

                if (fromConfiguration)
                    return Ok(await AuthenticationService.OTPLogin(request, ipAddress, userAgent, fromConfiguration, Culture));
                else
                    return Ok(await AuthenticationService.Login(request, ipAddress, userAgent));


            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                AuthenticationService.Logout();
                return Ok("User has been logged out");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }
    }
}

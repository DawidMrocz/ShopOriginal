using Framework.ApiModels.Access.Request;
using Framework.Models.Authentication;
using System.Security.Cryptography;

namespace Framework.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<string> Login(LoginRequest dto, string? ipAddress, string? userAgent);
        void Logout();
        Task<AuthenticationUser?> GetAuthenticatedUser();
        Task<string> OTPLogin(LoginRequest dto, string? ipAddress, string? userAgent, bool twoFactorAuth, string culture);
        Task VerifyOneTimePasscode(Guid guid, string passcode);
    }
}

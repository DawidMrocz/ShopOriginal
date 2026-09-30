using AngleSharp.Dom;
using Framework.ApiModels.Access.Request;
using Framework.Configuration;
using Framework.Models.Authentication;
using Framework.Models.OneTimePasscode;
using Framework.Models.User;
using Framework.Repositories.OneTimePasscode;
using Framework.Repositories.User;
using Framework.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Framework.Services.Authentication
{
    public class AuthenticationService_Cookie<TUserBaseModel, TId> : IAuthenticationService
        where TUserBaseModel : UserBaseModel<TId>
        where TId : struct
    {
        public readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected const string LoggedUserKey = "UserSessionKey";
        private readonly IUserRepository<TUserBaseModel, TId> _userRepository;
        private readonly IOneTimePasscodeRepository<TId> _oneTimePasscodeRepository;
        public AuthenticationService_Cookie(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUserRepository<TUserBaseModel, TId> userRepository, IEmailService emailService, IOneTimePasscodeRepository<TId> oneTimePasscodeRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _userRepository = userRepository;
            _oneTimePasscodeRepository = oneTimePasscodeRepository;
            _emailService = emailService;
        }

        private string GeneratePasscode()
        {
            var charsTable = Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray();

            var r = new Random();
            var passcore = new StringBuilder();

            //for (int i = 0; i < _userConf.OneTimePasscodeLength; i++)
            for (int i = 0; i < 6; i++)
                passcore.Append(charsTable[r.Next(charsTable.Length)]);

            return passcore.ToString();
        }

        public async Task VerifyOneTimePasscode(Guid guid, string passcode)
        {
            OneTimePasscodeModel<TId>? correctPasscode = await _oneTimePasscodeRepository.GetByGuid(guid);

            if (correctPasscode is null || correctPasscode.ExpireDate <= DateTime.Now)
                throw new Exception("Podane hasło jednorazowe jest nieaktualne");

            _oneTimePasscodeRepository.Delete(correctPasscode);
       
            if (correctPasscode.Passcode != passcode)
                throw new Exception("Podane hasło jednorazowe jest nieprawidłowe");

            TUserBaseModel? user = await _userRepository.GetById(correctPasscode.UserId)
                ?? throw new Exception("User not found");

            string authToken = Guid.NewGuid().ToString();

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("CSRFProtection", authToken.ToString())
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);
            AuthenticationProperties authenticationProperties = new()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.AddMinutes(FrameworkConfiguration.SessionTimeout),
                IsPersistent = correctPasscode.IsAuthTokenPersistent,
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: principal,
                properties: authenticationProperties
            );

            _httpContextAccessor.HttpContext!.User = principal;

            AuthenticationUser? authenticationUser = new()
            {
                Id = user.Id.ToString(),
                AuthorizeToken = authToken ?? Guid.NewGuid().ToString(),
                Role = user.Role.ToString()
            };

            _httpContextAccessor.HttpContext.Session.SetString(LoggedUserKey, JsonConvert.SerializeObject(authenticationUser));
        }

        public async Task<string> OTPLogin(LoginRequest dto, string? ipAddress, string? userAgent, bool twoFactorAuth, string culture) 
        {
            TUserBaseModel? user = await _userRepository.GetByEmail(dto.Email)
                ?? throw new Exception("Nie znaleziono użytkownika lub nie poprawne hasło");

            if (user.UnblockTime is null)
                if (user.Blocked is not null && user.Blocked == true)
                    throw new Exception("Knoto zostało zablokowane");
                else
                if (user.UnblockTime > DateTime.Now)
                    if (user.Blocked is not null && user.Blocked == true)
                        throw new Exception("Knoto zostało zablokowane");

            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Nie poprawne hasło");


            string passcode = GeneratePasscode();

            List<MailAddress> mailAddresses = [new MailAddress(user.Email)];

            StringBuilder title = new();

            if (user.FirstName is null)
            {
                title.Append("użytkowniku");
            }
            else
            {
                title.Append($"{user.FirstName} ");
                if (user.LastName is not null)
                    title.Append(user.LastName);
            }

            Dictionary<string, string> bodyParmas = new()
                {
                    { "UserName", title.ToString() },
                    { "Passcode", passcode }
                };

            Dictionary<string, string> subjectParmas = new()
                {
                    { "UserName", title.ToString() },
                };

            await _emailService.SendEmail(mailAddresses, "ToFactorAuth", true, "pl", new MailAddress(FrameworkConfiguration.User), bodyParmas, subjectParmas);

            //DODAC TABELE V
            var passcodeInsert = new OneTimePasscodeModel<TId>
            {
                Passcode = passcode,
                UserId = user.Id,
                // Z CONFIGURACJI
                //ExpireDate = DateTime.Now.AddMinutes(_userConf.OneTimePasscodeValidFor),
                ExpireDate = DateTime.Now.AddMinutes(6),
                IsAuthTokenPersistent = dto.RememberMe
            };
            int id = await _oneTimePasscodeRepository.Create(passcodeInsert);
            OneTimePasscodeModel<TId> otp = await _oneTimePasscodeRepository.GetById(id)
                ?? throw new Exception("Not found");

            return otp.UserId.ToString();
        }

        public async Task<string> Login(LoginRequest dto, string? ipAddress, string? userAgent)
        {
            TUserBaseModel? user = await _userRepository.GetByEmail(dto.Email)
                ?? throw new Exception("Nie znaleziono użytkownika lub nie poprawne hasło");

            if (user.UnblockTime is null)
                if (user.Blocked is not null && user.Blocked == true)
                    throw new Exception("Knoto zostało zablokowane");
                else
                if (user.UnblockTime > DateTime.Now)
                    if (user.Blocked is not null && user.Blocked == true)
                        throw new Exception("Knoto zostało zablokowane");

            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Nie poprawne hasło");           

            string authToken = Guid.NewGuid().ToString();

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("CSRFProtection", authToken.ToString())
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);
            AuthenticationProperties authenticationProperties = new()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.AddMinutes(FrameworkConfiguration.SessionTimeout),
                IsPersistent = dto.RememberMe
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: principal,
                properties: authenticationProperties
            );

            _httpContextAccessor.HttpContext!.User = principal;

            AuthenticationUser? authenticationUser = new()
            {
                Id = user.Id.ToString(),
                AuthorizeToken = authToken ?? Guid.NewGuid().ToString(),
                Role = user.Role.ToString()
            };

            _httpContextAccessor.HttpContext.Session.SetString(LoggedUserKey, JsonConvert.SerializeObject(authenticationUser));

            return "Pomyślnie zalogowano";
        }
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(LoggedUserKey);
            _httpContextAccessor.HttpContext?.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public virtual async Task<AuthenticationUser?> GetAuthenticatedUser()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            IIdentity? userIdentity = httpContext.User.Identity;

            if (userIdentity is null || !userIdentity.IsAuthenticated) return default;
            if (userIdentity is WindowsIdentity) return default;
            if (userIdentity is not ClaimsIdentity) return default;

            TId? userIdentifier = (TId?)Convert.ChangeType(((ClaimsIdentity)userIdentity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, typeof(TId));
            string? CSRFProtectionToken = ((ClaimsIdentity)userIdentity).Claims.FirstOrDefault(x => x.Type == "CSRFProtection")?.Value;

            AuthenticationUser? userFromSession;

            if (httpContext.Session.Keys.Contains(LoggedUserKey))
            {
                string? value = _httpContextAccessor.HttpContext?.Session.GetString(LoggedUserKey);
                JsonSerializerSettings settings = new();
                settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                userFromSession = value is null ? default : JsonConvert.DeserializeObject<AuthenticationUser>(value, settings);
            }
            else
            {
                userFromSession = default;
            }

            bool isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

            if (userFromSession != null && isAuthenticated && userIdentifier!.Equals(userFromSession.Id))
                return userFromSession;

            if (userIdentifier is null) return default;

            TUserBaseModel user = await _userRepository.GetById((TId)userIdentifier)
                ?? throw new Exception("User not found");

            AuthenticationUser? authenticationUser = new()
            {
                Id = user.Id.ToString(),
                AuthorizeToken = CSRFProtectionToken ?? Guid.NewGuid().ToString(),
                Role = user.Role.ToString()
            };
            httpContext.Session.SetString(LoggedUserKey, JsonConvert.SerializeObject(authenticationUser));
            return authenticationUser;

        }

        public async Task RefreshSession()
        {
            AuthenticationUser? authenticatedUser = await GetAuthenticatedUser();
            if (authenticatedUser is not null)
            {
                TUserBaseModel user = await _userRepository.GetById((TId)Convert.ChangeType(authenticatedUser.Id, typeof(TId)))
                ?? throw new Exception("User not found");

                AuthenticationUser? authenticationUser = new()
                {
                    Id = user.Id.ToString(),
                    AuthorizeToken = authenticatedUser.AuthorizeToken ?? Guid.NewGuid().ToString(),
                    Role = user.Role.ToString()
                };
                _httpContextAccessor.HttpContext?.Session.SetString(LoggedUserKey, JsonConvert.SerializeObject(authenticationUser));
            }
        }
    }
}

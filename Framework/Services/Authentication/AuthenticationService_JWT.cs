using Framework.ApiModels.Access.Request;
using Framework.Configuration;
using Framework.Models.Authentication;
using Framework.Models.OneTimePasscode;
using Framework.Models.Token;
using Framework.Models.User;
using Framework.Repositories.Token;
using Framework.Repositories.User;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authentication;

//using Framework.Shared.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Framework.Repositories.OneTimePasscode;
using Framework.Services.Email;

namespace Framework.Services.Authentication
{
    public class AuthenticationService_JWT<TUserBaseModel, TId> : IAuthenticationService
        where TUserBaseModel : UserBaseModel<TId>
    {
        private readonly IUserRepository<TUserBaseModel, TId> _userRepository;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ITokenRepository<TId> _tokenRepository;
        protected readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private static ConcurrentDictionary<string, object> _userLock = new();
        private readonly IEmailService _emailService;
        private readonly IOneTimePasscodeRepository<TId> _oneTimePasscodeRepository;

        //Ten provider jest scoped, możemy w nim zapisać prywatną zmienną z tokenem, żeby po SignIn GetAuthenticatedUser coś zwrócił
        private const string _tokenSessionKey = "JwtTempToken";

        public AuthenticationService_JWT(
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IUserRepository<TUserBaseModel, TId> userRepository,
            ITokenRepository<TId> tokenRepository,
            IEmailService emailService,
            IOneTimePasscodeRepository<TId> oneTimePasscodeRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _oneTimePasscodeRepository = oneTimePasscodeRepository;
        }
        public async Task<AuthenticationUser?> GetAuthenticatedUser()
        {
            string jwtToken = GetJwtToken();
            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                Logout();
                return default;
            }

            //RSA rsa = RSA.Create();
            //rsa.ImportRSAPublicKey(
            //    source: Convert.FromBase64String(FrameworkConfiguration.JwtPublicKey!),
            //    bytesRead: out int _
            //);

            //RsaSecurityKey rsaPublicKey = new(rsa);
            //string? jwtKey = _configuration.GetSection("Options:JwtPublicKey").Value;

            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken decryptedToken = tokenHandler.ReadJwtToken(jwtToken);



            //var key = Encoding.ASCII.GetBytes(FrameworkConfiguration.JwtPrivateKey!);

            //var validations = new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = new SymmetricSecurityKey(key),
            //    ValidateIssuer = false,
            //    ValidateAudience = false
            //};
            //var claims = tokenHandler.ValidateToken(jwtToken, validations, out var tokenSecure);

            string userId = decryptedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string userRole = decryptedToken.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            if (decryptedToken.ValidTo < DateTime.UtcNow)
            {
                //Nieważny token, trzeba odświeżyć za pomocą refreshToken
                string? refreshTokenValue = _httpContextAccessor.HttpContext?.Request.Cookies[FrameworkConfiguration.JwtRefreshTokenCookieName!]; ;
                if (string.IsNullOrWhiteSpace(refreshTokenValue))
                {
                    //Nie ma nawet tokena
                    Logout();
                    return default;
                }
                //Generujemy lub pobieramy lock dla aktualnego usera, żeby wiele requestów naraz nie spamowało refresh tokenów
                var lockObj = _userLock.GetOrAdd(userId, new object());

                lock (lockObj)
                {
                    if (!_memoryCache.TryGetValue($"RefreshToken-{refreshTokenValue}", out jwtToken!))
                    {
                        TokenModel<TId>? currentRefreshToken = _tokenRepository.GetTokenByValue(refreshTokenValue).Result; // w sql czasowy offset
                        if (userId != currentRefreshToken?.UserId.ToString() || currentRefreshToken?.ExpireDate <= DateTime.Now)
                        {
                            //Token nie jest dla user dla którego mamy JWT albo jest przeterminowany
                            Logout();
                            return default;
                        }
                        jwtToken = RefreshToken(currentRefreshToken!, userId, userRole).Result;
                        _memoryCache.Set($"RefreshToken-{refreshTokenValue}", jwtToken, absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(1));
                    }
                }
            }

            //Implementacja SlidingExpiration dla ciastka JWT
            if (!string.IsNullOrWhiteSpace(FrameworkConfiguration.JwtRefreshTokenCookieName!))
            {
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(
               FrameworkConfiguration.CookieName,

               jwtToken,
               new CookieOptions()
               {
                   HttpOnly = FrameworkConfiguration.CookieHttpOnly,
                   SameSite = SameSiteMode.Lax,
                   Expires = DateTime.Now.AddMinutes(FrameworkConfiguration.SessionTimeout),
                   IsEssential = true,
                   Secure = ShouldCreateSecureCookie()
               }
           );
            }



            //tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            //{
            //    IssuerSigningKey = rsaPublicKey,
            //    ValidateAudience = false,
            //    ValidateIssuer = false,
            //}, out var _);

            //var user = JsonConvert.DeserializeObject<AuthenticationUser?>(userJson.Value);
            //return new AuthenticationUser()
            //{
            //    Id = int.Parse(userId),
            //    Role = userRole,
            //};

            return new AuthenticationUser()
            {
                Id = userId,
                Role = userRole,
            };
        }

        private async Task<string> RefreshToken(TokenModel<TId> refreshToken, string userId, string role)
        {
            //Generujemy nowy jwt token
            string token = CreateToken(userId, role);
            // _jwtTokenDictForRefresh.AddOrUpdate(userIdentifier, token, (key, oldValue) => token);
            //Usuwamy użyty refresh token
            await _tokenRepository.DeleteToken(refreshToken.Id);
            //Tworzymy nowy refresh token
            //await CreateRefreshToken(int.Parse(refreshToken.UserId.ToString()));
            await CreateRefreshToken((TId)Convert.ChangeType(refreshToken.UserId, typeof(TId))!);

            return token;
        }

        private string GetJwtToken()
        {
            string token;
            //Najpierw próbujemy z nagłówków
            string? authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            Regex jwtAuthenticationTokenRegex = new Regex("^(bearer |Bearer )?(?<Bearer>.*)$");
            Match m = jwtAuthenticationTokenRegex.Match(authHeader ?? string.Empty);
            if (m.Success) token = m.Groups["Bearer"].ToString();

            var value = _httpContextAccessor.HttpContext?.Session.GetString(_tokenSessionKey);
            //MT musi tak być bo inaczej przy budowaniu listy w viemModelu lista jest apendowana przez co dublikuje elementy
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            //token = value == null ? default : JsonConvert.DeserializeObject<string>(value, settings);

            if (!string.IsNullOrWhiteSpace(value)) return value;

            //Jak nie ma to może z ciastka
            return _httpContextAccessor.HttpContext?.Request.Cookies[FrameworkConfiguration.CookieName]!;
        }
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new(passwordSalt))
            {
                byte[]? computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(string userId, string role)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role,role),
            };

            var signingCredentials = new SigningCredentials(
                   key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dawddawddawddawddawddawddawddawd")),
                   algorithm: SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                    audience: FrameworkConfiguration.Audience,
                    issuer: FrameworkConfiguration.Issuer,
                    claims: claims,
                    notBefore: null,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: signingCredentials
                );



            //string? token;

            //using (RSA rsa = RSA.Create())
            //{
            //    rsa.ImportRSAPrivateKey(
            //        source: Convert.FromBase64String(FrameworkConfiguration.JwtPrivateKey!),
            //        bytesRead: out int _);
            //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(FrameworkConfiguration.JwtPrivateKey!));

            //    var signingCredentials = new SigningCredentials(
            //       key: new RsaSecurityKey(rsa),
            //       algorithm: SecurityAlgorithms.RsaSha256)
            //    {
            //        CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            //    };


            //    var jwt = new JwtSecurityToken(
            //        //audience: FrameworkConfiguration.Audience,
            //        //issuer: FrameworkConfiguration.Issuer,
            //        audience:  null,
            //        issuer: null,
            //        claims: claims,
            //        notBefore: null,
            //        expires: FrameworkConfiguration.JwtTokenLifetime != null ? DateTime.UtcNow.AddHours((double)FrameworkConfiguration.JwtTokenLifetime) : DateTime.UtcNow.AddHours(1),
            //        signingCredentials: signingCredentials
            //    );

            //

            // jwt.Payload[payloadKey] = payload; userJSON

            //return new JwtSecurityTokenHandler().WriteToken(jwt);
            string? token = new JwtSecurityTokenHandler().WriteToken(jwt);
            //}

            _httpContextAccessor.HttpContext?.Session.SetString(_tokenSessionKey, token);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
               FrameworkConfiguration.CookieName,

               token,
               new CookieOptions()
               {
                   HttpOnly = FrameworkConfiguration.CookieHttpOnly,
                   SameSite = SameSiteMode.Lax,
                   Expires = DateTime.Now.AddMinutes(FrameworkConfiguration.SessionTimeout),
                   IsEssential = true,
                   Secure = ShouldCreateSecureCookie()
               }
           );

            //_httpContextAccessor.HttpContext?.Response.Cookies.Append(
            //    FrameworkConfiguration.JwtRefreshTokenCookieName!,
            //    token,
            //    new CookieOptions()
            //    {
            //        HttpOnly = FrameworkConfiguration.CookieHttpOnly,
            //        SameSite = SameSiteMode.Lax,
            //        Expires = DateTime.Now.AddMinutes(FrameworkConfiguration.SessionTimeout),
            //        IsEssential = true,
            //        Secure = Enum.Parse<CookieSecurePolicy>(FrameworkConfiguration.CookieSecurePolicy) switch
            //        {
            //            CookieSecurePolicy.Always => true,
            //            CookieSecurePolicy.None => false,
            //            CookieSecurePolicy.SameAsRequest => _httpContextAccessor.HttpContext.Request.IsHttps,
            //            _ => false
            //        }
            //    }
            //);
            return token;
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

            string token = CreateToken(user.Id.ToString(), user.Role.ToString());

            await CreateRefreshToken(user.Id);
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
            //SZUKAMY USERA
            TUserBaseModel? user = await _userRepository.GetByEmail(dto.Email) ?? throw new Exception("Nie znaleziono użytkownika lub nie poprawne hasło");

            if (user.UnblockTime is null)
                if (user.Blocked is not null && user.Blocked == true)
                    throw new Exception("Knoto zostało zablokowane");
                else
                if (user.UnblockTime > DateTime.Now)
                    if (user.Blocked is not null && user.Blocked == true)
                        throw new Exception("Knoto zostało zablokowane");

            // WALIADCJA HASŁA
            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Nie poprawne hasło");

            string token = CreateToken(user.Id.ToString(), user.Role.ToString());
       
            await CreateRefreshToken(user.Id);

            return token;
        }

        public void Logout()
        {
            //Usuwamy ciastko
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(FrameworkConfiguration.CookieName);

            //Usuwamy refresh token
            if (!string.IsNullOrWhiteSpace(FrameworkConfiguration.JwtRefreshTokenCookieName!))
                _httpContextAccessor.HttpContext?.Response.Cookies.Delete(FrameworkConfiguration.JwtRefreshTokenCookieName!);

            //Usuwamy z sesji tmp JWT
            _httpContextAccessor.HttpContext?.Session.Remove(_tokenSessionKey);
        }

        private async Task CreateRefreshToken(TId userIdentifier)
        {
            var refreshExpireDate = DateTime.Now.AddMinutes((double)FrameworkConfiguration.JwtRefreshTokenLifetimeInMinutes!);

            TokenModel<TId> refreshToken = new()
            {
                UserId = (TId)Convert.ChangeType(userIdentifier, typeof(TId))!,
                Value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpireDate = refreshExpireDate
            };

            await _tokenRepository.CreateToken(refreshToken);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                FrameworkConfiguration.JwtRefreshTokenCookieName!,
                refreshToken.Value,
                new CookieOptions()
                {
                    HttpOnly = FrameworkConfiguration.CookieHttpOnly,
                    SameSite = SameSiteMode.Lax,
                    Expires = refreshExpireDate,
                    IsEssential = true,
                    Secure = ShouldCreateSecureCookie()
                }
            );
        }


        private bool ShouldCreateSecureCookie()
        {
            var securePolicy = Enum.Parse<CookieSecurePolicy>(FrameworkConfiguration.CookieSameSite);
            return securePolicy switch
            {
                CookieSecurePolicy.Always => true,
                CookieSecurePolicy.None => false,
                CookieSecurePolicy.SameAsRequest => _httpContextAccessor.HttpContext!.Request.IsHttps,
                _ => false
            };
        }
    }
}

using Framework.Configuration;
using Framework.Services.Certificate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Framework.Extensions
{
    public static class AuthorizationProviderExtension
    {
        public static AuthenticationBuilder AuthorizationProvider(this IServiceCollection services, string authorizationProviderEnum)
        {
            AuthenticationBuilder authBuilder;
            switch (authorizationProviderEnum)
            {
                case "Cookie":

                    services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("Klucz zaoisettings"))
                   .SetApplicationName("Nazwa appki z appsetings")
                   .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

                    //services.AddAuthentication(options =>
                    //{
                    //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    //}).AddCookie().AddOpenIdConnect()


                    authBuilder = services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    }).AddCookie(options =>
                    {
                        options.Cookie.Name = FrameworkConfiguration.CookieName;
                        options.Cookie.Path = "/";
                        options.Cookie.SameSite = Enum.Parse<SameSiteMode>(FrameworkConfiguration.CookieSameSite);
                        options.Cookie.SecurePolicy = Enum.Parse<CookieSecurePolicy>(FrameworkConfiguration.CookieSecurePolicy);
                        options.Cookie.HttpOnly = FrameworkConfiguration.CookieHttpOnly;
                        options.Cookie.Domain = FrameworkConfiguration.DomainToSaveCookies;
                        options.Events = new CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = 401;
                                return Task.CompletedTask;
                            }
                        };
                    });
                    break;

                case "JWT":
                    authBuilder = services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                        {
                            options.RequireHttpsMetadata = false;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateAudience = false,
                                ValidAudience = FrameworkConfiguration.Issuer,
                                ValidateIssuer = true,
                                ValidIssuers = new[] { FrameworkConfiguration.Issuer },
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(FrameworkConfiguration.JwtPrivateKey!)),
                                RequireExpirationTime = true,
                                ValidateLifetime = true,
                                RequireSignedTokens = true,
                            };
                            options.Events = new JwtBearerEvents();
                            options.Events.OnMessageReceived = (context) =>
                            {
                                context.Token = context.Request.Cookies[FrameworkConfiguration.CookieName];
                                return Task.CompletedTask;
                            };
                            options.Events.OnAuthenticationFailed = (c) =>
                            {
                                c.Response.WriteAsJsonAsync("Mroczware.Framework.Api.JWT.Token.Validation.Fail").Wait();
                                return Task.CompletedTask;
                            };
                            options.Events.OnTokenValidated = context =>
                            {
                                if (!context.Principal.Identity.IsAuthenticated)
                                    context.Response.WriteAsJsonAsync("Mroczware.Framework.Api.JWT.Token.Validation.Fail").Wait();

                                return Task.CompletedTask;
                            };
                        });
                    break;
                case "Certificate":
                    authBuilder = services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                        .AddCertificate(options =>
                        {
                            options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                            options.Events = new CertificateAuthenticationEvents
                            {
                                OnCertificateValidated = context =>
                                {
                                    var validationService = context.HttpContext.RequestServices.GetService<ICertificateValidationService>();
                                    if (validationService.ValidateCertificate(context))
                                    {
                                        Console.WriteLine("Walidated successfuly");
                                        context.Success();
                                    }
                                    else
                                    {
                                        context.Fail("Invalid certificate");
                                    }
                                    return Task.CompletedTask;
                                },
                                OnAuthenticationFailed = context =>
                                {
                                    context.Fail("Invalid certificate");
                                    return Task.CompletedTask;
                                }
                            };
                        })

                        //.AddCertificateCache(options =>
                        //{
                        //    options.CacheSize = 1024;
                        //    options.CacheEntryExpiration = TimeSpan.FromMinutes(2);
                        //})
                        ;
                    break;
                default:
                    throw new Exception("Nie wybrano uwierzytelnienia");
                    
                    //case "SSO":
                    //    //if (Configuration.UseSqlDataProtection)
                    //    //    services.AddDataProtection()
                    //    //        .PersistKeysToDbContext<SsoKeysContext>()
                    //    //        .SetApplicationName(Configuration.ApplicationName)
                    //    //        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
                    //    //else
                    //    //    services.AddDataProtection()
                    //    //        .PersistKeysToFileSystem(new DirectoryInfo(Configuration.KeyStorePath))
                    //    //        .SetApplicationName(Configuration.ApplicationName)
                    //    //        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));


                    //    var defualtScheme = "Cookies_OR" + JwtBearerDefaults.AuthenticationScheme + Configuration.SsoScheme;
                    //    IdentityModelEventSource.ShowPII = true;
                    //    services.AddAuthentication(options =>
                    //    {
                    //        options.DefaultScheme = defualtScheme;
                    //        options.DefaultChallengeScheme = defualtScheme;
                    //    })
                    //    .AddCookie("Cookies", options =>
                    //    {
                    //        options.AccessDeniedPath = "/account/denied";
                    //        options.Cookie.Name = FrameworkConfiguration.CookieName;
                    //        options.Cookie.Path = "/";
                    //        options.Cookie.SameSite = SameSiteMode.Lax;
                    //        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    //        options.Events = new CookieAuthenticationEvents
                    //        {
                    //            OnRedirectToLogin = redirectContext =>
                    //            {
                    //                redirectContext.HttpContext.Response.StatusCode = 401;
                    //                return Task.CompletedTask;
                    //            }
                    //        };
                    //    })
                    //    //.MroczwareSsoAddJwtBearer()
                    //    .AddOpenIdConnect(Configuration.SsoScheme, options =>
                    //    {
                    //        options.SignInScheme = "Cookies";

                    //        options.Authority = Configuration.SsoAuthority;
                    //        options.ClientId = Configuration.SsoClientId;
                    //        options.ClientSecret = Configuration.SsoClientSecret;
                    //        options.CorrelationCookie.SameSite = SameSiteMode.Lax;

                    //        options.BackchannelHttpHandler = new HttpClientHandler()
                    //        {
                    //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,

                    //        };
                    //        options.ResponseType = OpenIdConnectResponseType.Code;
                    //        options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;

                    //        options.SaveTokens = true;
                    //        options.GetClaimsFromUserInfoEndpoint = true;
                    //        var scopes = Configuration.SsoScope.Split(',');
                    //        foreach (var scope in scopes)
                    //            options.Scope.Add(scope);

                    //        //options.TokenValidationParameters.NameClaimType = "Name";
                    //        //options.TokenValidationParameters.RoleClaimType = "Role";
                    //        //options.TokenValidationParameters.IssuerSigningKey=new SymmetricSecurityKey( Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY="));
                    //        //options.TokenValidationParameters.ValidateActor = false;
                    //        //options.TokenValidationParameters.ValidateAudience = false;
                    //        //options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    //        options.TokenValidationParameters = Configuration.SsoJwtConfiguration.GetTokenValidationParameters();
                    //        options.RequireHttpsMetadata = false;

                    //        //Domyślnie jeżeli brak Settings - jest: @"/" - czyli strona głowna aplikacji
                    //        options.SignedOutCallbackPath = Configuration.SsoSignedOutCallbackPath;
                    //        options.Events.OnTokenValidated = context =>
                    //        {
                    //            var authenticationService = context.HttpContext.RequestServices.GetService(typeof(IAuthenticationService)) as IAuthenticationService;
                    //            if (context.Principal.Identity.IsAuthenticated)
                    //            {
                    //                var rolesClaim = context.Principal.Claims.FirstOrDefault(x => x.Type == "Roles");
                    //                var roles = new string[0];
                    //                if (rolesClaim != null && !string.IsNullOrEmpty(rolesClaim.Value))
                    //                    roles = rolesClaim.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    //                var importUserDto = new ImportUserDto
                    //                {
                    //                    Login = context.Principal.Claims.First(x => x.Type == "Login").Value,
                    //                    FirstName = context.Principal.Claims.First(x => x.Type == "FirstName").Value,
                    //                    LastName = context.Principal.Claims.First(x => x.Type == "LastName").Value,
                    //                    ExternalId = context.Principal.Claims.First(x => x.Type == "Guid").Value,
                    //                    Roles = roles
                    //                };

                    //                var uid = context.Principal.Claims.First(x => x.Type == "sub");
                    //                var userGuidString = importUserDto.ExternalId;
                    //                if (userGuidString != null && uid != null && importUserDto != null && importUserDto.Login != null)
                    //                    authenticationService.SignIn(userGuidString, importUserDto.Login, false, importUserDto);
                    //            }
                    //            return Task.CompletedTask;
                    //        };
                    //        options.Events.OnRedirectToIdentityProviderForSignOut = context =>
                    //        {
                    //            var authenticationService = context.HttpContext.RequestServices.GetService(typeof(IAuthenticationService)) as IAuthenticationService;
                    //            authenticationService.SignOut();
                    //            return Task.CompletedTask;
                    //        };
                    //        options.Events.OnAuthorizationCodeReceived = context =>
                    //        {
                    //            context.TokenEndpointRequest.SetParameter("id_token_key_type", "JWK");
                    //            return Task.CompletedTask;
                    //        };

                    //    })
                    //    .AddPolicyScheme(defualtScheme, defualtScheme, options =>
                    //    {
                    //        options.ForwardDefaultSelector = context =>
                    //        {
                    //            string authorization = context.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization];
                    //            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                    //            {
                    //                return JwtBearerDefaults.AuthenticationScheme;
                    //            }
                    //            return Configuration.SsoScheme;
                    //        };
                    //    });
                    //    break;
            };
            return authBuilder;
        }
    }
}
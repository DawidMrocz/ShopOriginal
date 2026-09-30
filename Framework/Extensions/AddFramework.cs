
using Framework.Client;
using Framework.Configuration;
using Framework.Enums;
using Framework.Middlewares;
using Framework.Models.Country;
using Framework.Models.Culture;
using Framework.Models.OneTimePasscode;
using Framework.Models.User;
using Framework.Repositories.Country;
using Framework.Repositories.Culture;
using Framework.Repositories.Currency;
using Framework.Repositories.File;
using Framework.Repositories.OneTimePasscode;
using Framework.Repositories.Setting;
using Framework.Repositories.Template;
using Framework.Repositories.Token;
using Framework.Repositories.User;
using Framework.Services.Authentication;
using Framework.Services.Email;
using Framework.Services.User;
using Framework.Shared.Services.EmailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IAuthenticationService = Framework.Services.Authentication.IAuthenticationService;

namespace Framework.Extensions
{

    public class HttpClientDto
    {
        public string Name { get; set; } = null!;
        [Url]
        public string BaseUrl { get; set; } = null!;
        public Certificate? Certificate { get; set; }
    }
    public class SSOClientDto
    {
        public string ClientName { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string Authority { get; set; } = null!;
        public string CallbackPath { get; set; } = null!;
        public string ResponseType { get; set; } = null!;
        public bool SaveTokens { get; set; }
        public string? NameClaimType { get; set; }
        public List<string> Scopes { get; set; } = new();
        public string RedirectAfterLogon { get; set; }
    }
    public static class AddFrameworkExtension
    {
        public static IApplicationBuilder AddFramework(this WebApplication app)
        {


            app.UseHttpsRedirection();
            app.UseRouting();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                Secure = Enum.Parse<CookieSecurePolicy>(FrameworkConfiguration.CookieSecurePolicy),
                HttpOnly = FrameworkConfiguration.CookieHttpOnly
               ? Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
               : Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None
            });

            app.UseSession();
            if (!string.IsNullOrEmpty(FrameworkConfiguration.AccessControlAllowOrigin))
            {
                app.UseCors(builder =>
                   builder.WithOrigins(FrameworkConfiguration.AccessControlAllowOrigin)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());
            }
            else
            {
                app.UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials()); // allow credentials
            }

            if (FrameworkConfiguration.CorsPolicies != null && FrameworkConfiguration.CorsPolicies.Any())
                foreach (var c in FrameworkConfiguration.CorsPolicies)
                    app.UseCors(c.Name);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            //app.UseMiddleware<AdminSafeListMiddleware>("127.0.0.1;192.168.1.5;::1;::ffff:172.21.0.1");
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            //app.UseEndpoints(endpoints => {
            //    endpoints.MapControllers();
            //});
            app.MapControllers();
            app.Run();

            return app;
        }



        public static IServiceCollection AddFramework<TDbContext, TUserModel, TUserId>(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
            where TUserModel : UserBaseModel<TUserId>, new()
            where TDbContext : DbContext, IUserRepositoryModels<TUserId>, IFileRepositoryModels, IEmailServiceModels, ISettingRepositoryModels, ICurrencyRepositoryModels, ICultureRepositoryModels, ICountryRepositoryModels, IOneTimePasscodeRepositoryModels<TUserId>
            where TUserId : struct
        {
            services.AddControllers().AddApplicationPart(typeof(AddFrameworkExtension).Assembly);
            services.AddEndpointsApiExplorer();


            //Zapisuje info o zadaniu http,nagłowkach treści  zadania i info o odpowiedzi
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;

            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
              });
            });

            FrameworkConfiguration.SetConfiguration(configuration, assembly.FullName);

            //List<HttpClientDto> httpClients = configuration.GetSection("HttpClients").Get<List<HttpClientDto>>()
            //    ?? throw new Exception("Appsetting not found");

            //foreach (HttpClientDto client in httpClients)
            //{
            //    services.AddHttpClient(client.Name, httpClient =>
            //    {
            //        if(client.Certificate is not null) 
            //        {
            //            HttpClientHandler handler = new();
            //            string certificateFullPath = Path.Combine(client.Certificate.Path, client.Certificate.Name);
            //            X509Certificate2 certificate = new(certificateFullPath, client.Certificate.Password);
            //            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //            handler.ClientCertificates.Add(certificate);
            //        }             
            //        httpClient.BaseAddress = new Uri(client.BaseUrl);
            //    });
            //}

            services.AddDependencyInjections(assembly);

            services.AddScoped<IUserRepository<TUserModel, TUserId>, UserRepository<TDbContext, TUserModel, TUserId>>();
            services.AddScoped<IFileRepository, FileRepository<TDbContext>>();

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(4);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "SessionCookie";
            });

            var corsPolicies = FrameworkConfiguration.CorsPolicies;

            if (corsPolicies != null && corsPolicies.Any())
            {
                services.AddCors(options =>
                {
                    foreach (var c in corsPolicies)
                    {
                        options.AddPolicy(name: c.Name,
                                buiilder =>
                                {
                                    if (c.Origins != null && c.Origins.Any())
                                    {
                                        buiilder.WithOrigins(c.Origins.ToArray());
                                    }
                                    else
                                    {
                                        buiilder.AllowAnyOrigin();
                                    }

                                    if (c.Methods != null && c.Methods.Any())
                                    {
                                        buiilder.WithMethods(c.Methods.ToArray());
                                    }
                                    else
                                    {
                                        buiilder.AllowAnyMethod();
                                    }

                                    if (c.Headers != null && c.Headers.Any())
                                    {
                                        buiilder.WithHeaders(c.Headers.ToArray());
                                    }
                                    else
                                    {
                                        buiilder.AllowAnyHeader();
                                    }

                                    if (c.ExposeHeaders != null && c.ExposeHeaders.Any())
                                    {
                                        buiilder.WithExposedHeaders(c.ExposeHeaders.ToArray());
                                    }

                                    if (c.AllowCredentials)
                                    {
                                        buiilder.AllowCredentials();
                                    }
                                    else
                                    {
                                        buiilder.DisallowCredentials();
                                    }
                                }

                            );
                    }
                });
            }
            else
            {
                services.AddCors();
            }

            //https://learn.microsoft.com/pl-pl/aspnet/core/security/samesite?view=aspnetcore-8.0
            //===================================================================
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            void CheckSameSite(HttpContext httpContext, CookieOptions options)
            {
                if (options.SameSite == Microsoft.AspNetCore.Http.SameSiteMode.None)
                {
                    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                    if (DisallowsSameSiteNone(userAgent, FrameworkConfiguration.DisallowsSameSiteNone))
                    {
                        options.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                    }
                }
            }
            //===================================================================


            services.AddScoped<ISettingRepository, SettingRepository<TDbContext>>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository<TDbContext>>();
            services.AddScoped<ICountryRepository<CountryModel, int>, CountryRepository<TDbContext>>();
            services.AddScoped<IOneTimePasscodeRepository<TUserId>, OneTimePasscodeRepository<TDbContext, TUserId>>();
            services.AddScoped<ICultureRepository<CultureModel, int>, CultureRepository<TDbContext>>();
            services.AddScoped<ITemplateRepository, TemplateRepository<TDbContext>>();
            services.AddScoped<IEmailService, EmailService<TDbContext>>();
            services.AddScoped<ITokenRepository<TUserId>, TokenRepository<TDbContext, TUserId>>();
            services.AddScoped<IUserService<TUserModel, TUserId>, UserService<TUserModel, TUserId>>();

            services.AddKeyedScoped<IAuthenticationService, AuthenticationService_Cookie<TUserModel, TUserId>>(AuthorizationProviderEnum.Cookie.ToString());
            services.AddKeyedScoped<IAuthenticationService, AuthenticationService_JWT<TUserModel, TUserId>>(AuthorizationProviderEnum.JWT.ToString());

            AuthenticationBuilder authBuilder = services.AuthorizationProvider(FrameworkConfiguration.AuthorizationProvider);

            //List<SSOClientDto> ssoClients = configuration.GetSection("SSO").Get<List<SSOClientDto>>()
            //    ?? throw new Exception("Appsetting not found");

            //foreach (SSOClientDto client in ssoClients)
            //{
            //    authBuilder.AddOpenIdConnect(client.ClientName, options =>
            //     {
            //         options.Authority = client.Authority;
            //         options.ClientId = client.ClientId;
            //         options.ResponseType = client.ResponseType;
            //         options.CallbackPath = client.CallbackPath;
            //         //options.SignedOutRedirectUri = client.RedirectAfterLogon;
            //         foreach (string scope in client.Scopes)
            //             options.Scope.Add(scope);
                     
            //         if (client.NameClaimType is not null)
            //             options.TokenValidationParameters.NameClaimType = client.NameClaimType;
            //     });
            //}

            return services;
        }

        private static bool DisallowsSameSiteNone(string userAgent, string disallowsSameSiteNone)
        {

            if (!string.IsNullOrEmpty(disallowsSameSiteNone))
            {
                foreach (var item in disallowsSameSiteNone.Split(";"))
                {
                    if (userAgent.Contains(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class CorsPolicy
    {
        public string Name { get; set; }
        public bool AllowCredentials { get; set; }
        public List<string> Origins { get; set; }
        public List<string> Methods { get; set; }
        public List<string> Headers { get; set; }
        public List<string> ExposeHeaders { get; set; }
    }
}



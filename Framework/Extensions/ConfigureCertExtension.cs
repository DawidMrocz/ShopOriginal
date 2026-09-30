using Framework.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Extensions
{
    public static class ConfigureCertExtension
    {
        public static WebApplicationBuilder ConfigureCert(this WebApplicationBuilder app) 
        {
  
            var allowAnyClientCertificate = false;

            var allowedClientCertificateThumbprints = new List<string>();
            app.WebHost.ConfigureAppConfiguration(configBuilder =>
            {
                // compile a temporary configuration instance
                var configuration = configBuilder.Build();

                // Check config values as you would normally
                allowedClientCertificateThumbprints = new List<string>() { "8703926D56BF396717641E929F16C29DDCDC54BA" };
                if (allowedClientCertificateThumbprints != null)
                {
                    allowAnyClientCertificate = allowedClientCertificateThumbprints.Any();
                }
            });
            app.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(o =>
                { 
                o.ClientCertificateMode = FrameworkConfiguration.ClientCertificateMode;
                    if (allowAnyClientCertificate)
                    {
                        o.AllowAnyClientCertificate();
                        o.ClientCertificateValidation += (sender, e, b) =>
                        {
                            var cert = sender as X509Certificate2;
                            if (cert == null)
                            {
                                Console.WriteLine("ClientCertificateValidation - Cert is null");
                                return false;
                            }
                            if (allowedClientCertificateThumbprints.Contains(sender.Thumbprint))
                            {
                                Console.WriteLine("ClientCertificateValidation - zezwalam");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("ClientCertificateValidation - nie pozwalam na Thumbprint:" + sender.Thumbprint);
                                return false;
                            }
                        };
                    }
                });
            });

            return app;
        }
    }
}

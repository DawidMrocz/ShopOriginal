
using BackOfficeUserApp.Data;
using Framework.Extensions;
using MassTransit;
using System.Reflection;

namespace BackOfficeUserApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureCert();

            builder.Services.AddDbContext<UserContext>();

            builder.Services.AddFramework<UserContext, Models.User, Guid>(builder.Configuration, Assembly.GetExecutingAssembly());

            //builder.Services.AddHealthChecks().AddDbContextCheck<UserContext>();

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "user", includeNamespace: false));
                busConfigurator.UsingRabbitMq((context, busFactoryConfiguration) =>
                {
                    busFactoryConfiguration.Host(builder.Configuration["RabbitMqSettings:Uri"]);
                    busFactoryConfiguration.ConfigureEndpoints(context);
                });
            });

            WebApplication? app = builder.Build();

            app.AddFramework();
            app.UseHealthChecks(path: "/jaksieczujesz");
        }
    }
}

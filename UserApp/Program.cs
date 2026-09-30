
using Framework.Extensions;
using MassTransit;
using System.Reflection;
using UserApp.Data;
using UserApp.Models;
using UserApp.Repositories;

namespace UserApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureCert();

            builder.Services.AddDbContext<UserContext>();

            builder.Services.AddFramework<UserContext, User, Guid>(builder.Configuration, Assembly.GetExecutingAssembly());
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddGrpcClient<ProductAppGrpc.Product.ProductClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddHealthChecks().AddDbContextCheck<UserContext>();

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "backOfficeUser", includeNamespace: false));
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
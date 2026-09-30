
using BackOfficeProductApp.Models;
using MassTransit;
using Framework.Extensions;
using BackOfficeProductApp.Data;
using System.Reflection;
using ProductAppGrpc;

namespace BackOfficeProductApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ProductContext>();
            builder.Services.AddFramework<ProductContext, User, Guid>(builder.Configuration, Assembly.GetExecutingAssembly());

            builder.Services.AddGrpcClient<ProductAppGrpc.Product.ProductClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddGrpcClient<ProductAppGrpc.Producer.ProducerClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddGrpcClient<ProductAppGrpc.Category.CategoryClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                //busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "product", includeNamespace: false));
                busConfigurator.UsingRabbitMq((context, busFactoryConfiguration) =>
                {
                    busFactoryConfiguration.Host(builder.Configuration["RabbitMqSettings:Uri"]);
                    busFactoryConfiguration.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "product", includeNamespace: false));
                busConfigurator.UsingRabbitMq((context, busFactoryConfiguration) =>
                {
                    busFactoryConfiguration.Host(builder.Configuration["RabbitMqSettings:Uri"]);
                    busFactoryConfiguration.ConfigureEndpoints(context);
                });
            });

            WebApplication? app = builder.Build();
            app.AddFramework();
        }
    }
}

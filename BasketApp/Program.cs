
using BasketApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Framework.Extensions;
using BasketApp.Data;
using Framework.Middlewares;
using Framework.Middlewares.Resulty;
using MassTransit;
using ProductAppGrpc;
namespace BasketApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureCert();

            builder.Services.AddDbContext<BasketContext>();
            builder.Services.AddFramework<BasketContext, User, Guid>(builder.Configuration, Assembly.GetExecutingAssembly());

            builder.Services.AddGrpcClient<Product.ProductClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddGrpcClient<Producer.ProducerClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddGrpcClient<Category.CategoryClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddGrpcClient<ProductAppGrpc.Discount.DiscountClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddSingleton<IAuthorizationHandler, AutoryzacjaHandler>();

            builder.Services.AddSingleton<
                IAuthorizationMiddlewareResultHandler, SampleAuthorizationMiddlewareResultHandler>();

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                busConfigurator.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "basket", includeNamespace: false));
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

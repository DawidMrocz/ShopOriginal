using Framework.Extensions;
using MassTransit;
using ProductApp.Data;
using ProductApp.Models;
using ProductAppGrpc;
using System.Reflection;

namespace ProductApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ProductContext>();
            builder.Services.AddFramework<ProductContext, User, Guid>(builder.Configuration, Assembly.GetExecutingAssembly());

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

            builder.Services.AddGrpcClient<Discount.DiscountClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
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

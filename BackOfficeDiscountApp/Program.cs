using BackOfficeDiscountApp.Data;
using ProductAppGrpc;
using Framework.Extensions;
using MassTransit;
using System.Reflection;


namespace BackOfficeDiscountApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DiscountContext>();
            builder.Services.AddFramework<DiscountContext, Models.User, Guid>(builder.Configuration, Assembly.GetExecutingAssembly());

            builder.Services.AddGrpcClient<ProductAppGrpc.Product.ProductClient>(o =>
            {
                o.Address = new Uri(builder.Configuration.GetConnectionString("Grpc")!);
            });

            builder.Services.AddGrpcClient<ProductAppGrpc.Discount.DiscountClient>(o =>
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

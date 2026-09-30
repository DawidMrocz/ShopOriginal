using Framework.Repositories.File;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProductAppGrpc.Data;
using ProductAppGrpc.Services;

namespace ProductAppGrpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ProductContext>();

            builder.WebHost.ConfigureKestrel(options =>
            {
                // Setup a HTTP/2 endpoint without TLS.
                options.ListenAnyIP(80, o => o.Protocols = HttpProtocols.Http2);
                options.ListenAnyIP(443, o => o.Protocols = HttpProtocols.Http2);
            });

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IFileRepository, FileRepository<ProductContext>>();
            //builder.Services.AddScoped<IFileRepository, FileRepository<ProductContext>>();
            //builder.Services.AddScoped<IFileRepository, FileRepository<ProductContext>>();
            //builder.Services.AddScoped<IFileRepository, FileRepository<ProductContext>>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGrpcService<ProductService>();
            app.MapGrpcService<ProducerService>();
            app.MapGrpcService<CategoryService>();
            app.MapGrpcService<DiscountService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
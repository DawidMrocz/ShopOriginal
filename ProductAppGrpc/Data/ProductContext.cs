using Framework.Models.Currency;
using Framework.Models.File;
using Framework.Repositories.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;

namespace ProductAppGrpc.Data
{
    public class ProductContext : DbContext,
        IFileRepositoryModels
    {
        private readonly IConfiguration _configuration;
        public DbSet<Models.ProductModel> Products { get; set; }
        public DbSet<Models.CategoryModel> Categories { get; set; }
        public DbSet<Models.ProducerModel> Producers { get; set; }
        public DbSet<Models.DiscountModel> Discounts { get; set; }
        public DbSet<CurrencyModel> Currencies { get; set; }
        public DbSet<FileModel> Files { get; set; }

        public ProductContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                _configuration.GetConnectionString("DefaultConnection"),
                x => x.CommandTimeout(15)
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );

                //TO DELETE
                //----------------------------------------------------------------------------              
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
                //optionsBuilder.LogTo(new StreamWriter("EnitityFramework-logs.txt", append: true).WriteLine);
                optionsBuilder.AddInterceptors(new MeasureQueryInterceptor());
                //----------------------------------------------------------------------------
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductContext).Assembly);
            modelBuilder.HasDefaultSchema(Assembly.GetExecutingAssembly().GetName().Name!.ToLower());
        }

        public class MeasureQueryInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor
        {
            public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
            {
                Console.WriteLine("================================================================================================");
                Console.WriteLine(command.CommandText);
                Console.WriteLine($"Zapytanie wykonalo sie {eventData.Duration.TotalMilliseconds} ms");
                Console.WriteLine("================================================================================================");
                return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
            }
            public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
            {
                Console.WriteLine("================================================================================================");
                Console.WriteLine(command.CommandText);
                Console.WriteLine($"Zapytanie wykonalo sie {eventData.Duration.TotalMilliseconds} ms");
                Console.WriteLine("================================================================================================");
                return base.ReaderExecuted(command, eventData, result);
            }
        }
    }
}

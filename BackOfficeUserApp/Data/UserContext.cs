using BackOfficeUserApp.Models;
using Framework.Models.Country;
using Framework.Models.Culture;
using Framework.Models.Currency;
using Framework.Models.File;
using Framework.Models.OneTimePasscode;
using Framework.Models.Setting;
using Framework.Models.Template;
using Framework.Models.Token;
using Framework.Repositories.Country;
using Framework.Repositories.Culture;
using Framework.Repositories.Currency;
using Framework.Repositories.File;
using Framework.Repositories.OneTimePasscode;
using Framework.Repositories.Setting;
using Framework.Repositories.User;
using Framework.Shared.Services.EmailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Reflection;

namespace BackOfficeUserApp.Data
{
    public partial class UserContext :
        DbContext,
        IUserRepositoryModels<Guid>,
        IFileRepositoryModels,
        ISettingRepositoryModels,
        IEmailServiceModels,
        ICountryRepositoryModels,
        ICultureRepositoryModels,
        ICurrencyRepositoryModels,
        IOneTimePasscodeRepositoryModels<Guid>
    {
        private readonly IConfiguration _configuration;

        public DbSet<TokenModel<Guid>> Tokens { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<TemplateModel> Templates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<CultureModel> Cultures { get; set; }
        public DbSet<CountryModel> Countries { get; set; }
        public DbSet<CurrencyModel> Currencies { get; set; }
        public DbSet<OneTimePasscodeModel<Guid>> OneTimePasscodes { get; set; }

        public UserContext(IConfiguration configuration)
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
                optionsBuilder.LogTo(new StreamWriter("EnitityFramework-logs.txt", append: true).WriteLine);
                optionsBuilder.AddInterceptors(new MeasureQueryInterceptor());
                //----------------------------------------------------------------------------
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserContext).Assembly);
            modelBuilder.HasDefaultSchema(Assembly.GetExecutingAssembly().GetName().Name!.ToLower());
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public class MeasureQueryInterceptor : DbCommandInterceptor
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

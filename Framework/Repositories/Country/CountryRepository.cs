using Framework.Models.Country;
using Framework.Repositories.Culture;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Country
{
    public interface ICountryRepositoryModels
    {
        public DbSet<CountryModel> Countries { get; set; }
    }
    internal class CountryRepository<TDbContext> : GenericRepository<TDbContext, CountryModel, int>, ICountryRepository<CountryModel, int>
        where TDbContext : DbContext, ICountryRepositoryModels
    {
        public CountryRepository(TDbContext dbContext) : base(dbContext) { }
    }
}

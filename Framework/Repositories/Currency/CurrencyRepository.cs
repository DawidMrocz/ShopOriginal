using Framework.Models.Currency;
using Framework.Models.File;
using Framework.Repositories.Currency;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Currency
{
    public interface ICurrencyRepositoryModels
    {
        public DbSet<CurrencyModel> Currencies { get; set; }
    }
    public class CurrencyRepository<TDbContext> : ICurrencyRepository
        where TDbContext : DbContext, ICurrencyRepositoryModels
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<CurrencyModel> _dbSet;

        public CurrencyRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<CurrencyModel>();
        }

        public async Task<CurrencyModel> Get(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new Exception("Currency not found");
        }

        public async Task<CurrencyModel> GetByCode(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Code == name)
                ?? throw new Exception("Currency not found");
        }
    }
}

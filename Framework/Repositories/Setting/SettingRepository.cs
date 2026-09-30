using Framework.Models.Setting;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Setting
{
    public interface ISettingRepositoryModels
    {
        public DbSet<SettingModel> Settings { get; set; }
    }
    internal class SettingRepository<TDbContext> : ISettingRepository
        where TDbContext : DbContext, ISettingRepositoryModels
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<SettingModel> _dbSet;

        public SettingRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<SettingModel>();
        }
        public async Task<SettingModel?> Get(string key)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Key == key);
        }
    }
}

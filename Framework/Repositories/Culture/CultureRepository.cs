using Framework.Models.Culture;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Culture
{
    public interface ICultureRepositoryModels
    {
        public DbSet<CultureModel> Cultures { get; set; }
    }
    internal class CultureRepository<TDbContext> : GenericRepository<TDbContext, CultureModel, int>, ICultureRepository<CultureModel, int>
        where TDbContext : DbContext, ICultureRepositoryModels
    {
        public CultureRepository(TDbContext dbContext) : base(dbContext) { }
    }
}

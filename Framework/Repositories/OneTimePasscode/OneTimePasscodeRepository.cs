using Framework.Models.OneTimePasscode;
using Framework.Repositories.Culture;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.OneTimePasscode
{
    public interface IOneTimePasscodeRepositoryModels<TUserId>
    {
        public DbSet<OneTimePasscodeModel<TUserId>> OneTimePasscodes { get; set; }
    }
    internal class OneTimePasscodeRepository<TDbContext, TUserId> : GenericRepository<TDbContext, OneTimePasscodeModel<TUserId>, int>, IOneTimePasscodeRepository<TUserId>
        where TDbContext : DbContext, IOneTimePasscodeRepositoryModels<TUserId>
    {
        public OneTimePasscodeRepository(TDbContext dbContext) : base(dbContext)  { }

        public async Task<OneTimePasscodeModel<TUserId>?> GetByGuid(Guid guid)
        {
            return await _dbContext.OneTimePasscodes.FirstOrDefaultAsync(x => x.Guid == guid);
        }
    }

}

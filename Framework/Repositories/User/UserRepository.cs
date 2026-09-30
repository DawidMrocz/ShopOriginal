using Framework.Models.File;
using Framework.Models.Token;
using Framework.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.User
{
    public interface IUserRepositoryModels<TUserId>
    {
        public DbSet<TokenModel<TUserId>> Tokens { get; set; }
        public DbSet<FileModel> Files { get; set; }
    }

    public partial class UserRepository<TDbContext, TEntity, TId> : GenericRepository<TDbContext, TEntity, TId>, IUserRepository<TEntity, TId>
        where TDbContext : DbContext, IUserRepositoryModels<TId>
        where TEntity : UserBaseModel<TId>
    {
        public UserRepository(TDbContext dbContext) : base(dbContext) { }

        public async Task<TEntity?> GetByEmail(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}

using Framework.Models.Token;
using Framework.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Token
{
    //[DependencyInjection(typeof(ITokenRepository))]
    internal class TokenRepository<TDbContext, TUserId> : ITokenRepository<TUserId>
        where TDbContext : DbContext, IUserRepositoryModels<TUserId>
        where TUserId : struct
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<TokenModel<TUserId>> _dbSet;

        public TokenRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TokenModel<TUserId>>();
        }

        public async Task CreateToken(TokenModel<TUserId> refreshToken)
        {
            await _dbSet.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteToken(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenModel<TUserId>> GetTokenByValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}

using Framework.Models;

namespace Framework.Repositories.User
{
    public interface IUserRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : BaseModel<TId>
    {
        Task<TEntity?> GetByEmail(string email);
    }
}

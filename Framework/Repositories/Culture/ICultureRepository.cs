using Framework.Models;

namespace Framework.Repositories.Culture
{
    public interface ICultureRepository<TEntity, TId>
        : IGenericRepository<TEntity, TId>
        where TEntity : BaseModel<TId>
    { }
}

using Framework.Models;

namespace Framework.Repositories.Label
{
    public interface ILabelRepository<TEntity, TId>
        : IGenericRepository<TEntity, TId>
        where TEntity : BaseModel<TId>
    { }
}

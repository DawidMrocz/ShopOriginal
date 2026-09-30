using Framework.Models;

namespace Framework.Repositories.Country
{
    public interface ICountryRepository<TEntity, TId> 
        : IGenericRepository<TEntity, TId> 
        where TEntity : BaseModel<TId> { }
}

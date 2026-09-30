using Framework.Models;

namespace Framework.Repositories
{
    public interface IGenericRepository<TEntity, T> where TEntity : BaseModel<T>
    {
        Task<TEntity?> GetById(T id);
        Task<List<TEntity>> GetList();
        Task<T> Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}

using Framework.Models;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Framework.Repositories
{
    public abstract class GenericRepository<TDbContext, TEntity, T> : IGenericRepository<TEntity, T>
        where TDbContext : DbContext
        where TEntity : BaseModel<T>
    {
        protected readonly TDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        public GenericRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> GetList()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity?> GetById(T id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity.Id;
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);

            _dbSet.Remove(entityToDelete);
        }


        public virtual void Update(TEntity entityToUpdate)
        {
            if (_dbContext.Entry(entityToUpdate).State == EntityState.Detached)
                _dbContext.Attach(entityToUpdate);

            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}

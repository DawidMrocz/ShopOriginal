using Framework.Models.Label;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Label
{
    public interface ILabelRepositoryModels
    {
        public DbSet<LabelModel> Labels { get; set; }
    }
    internal class LabelRepository<TDbContext> : GenericRepository<TDbContext, LabelModel, int>, ILabelRepository<LabelModel, int>
        where TDbContext : DbContext, ILabelRepositoryModels
    {
        public LabelRepository(TDbContext dbContext) : base(dbContext) { }z
    }
}

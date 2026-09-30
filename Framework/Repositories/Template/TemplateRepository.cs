using Framework.Models.Template;
using Framework.Shared.Services.EmailService;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.Template
{
    internal class TemplateRepository<TDbContext> : ITemplateRepository
        where TDbContext : DbContext, IEmailServiceModels
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<TemplateModel> _dbSet;

        public TemplateRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TemplateModel>();
        }
        public async Task<TemplateModel?> Get(string templateName, int culture)
        {
            return await _dbSet.Where(t => t.Name == templateName && t.CultureId == culture).FirstOrDefaultAsync();
        }
    }
}

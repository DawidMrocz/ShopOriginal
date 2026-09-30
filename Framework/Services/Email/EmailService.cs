using Framework.Models.Template;
using Framework.Repositories.Setting;
using Framework.Repositories.Template;
using Framework.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace Framework.Shared.Services.EmailService
{
    public interface IEmailServiceModels
    {
        public DbSet<TemplateModel> Templates { get; set; }
    }

    internal partial class EmailService<TDbContext> : IEmailService
        where TDbContext : DbContext, IEmailServiceModels
    {
        private readonly DbSet<TemplateModel> _dbSet;
        private readonly ISettingRepository _settingRepository;
        private readonly TDbContext _dbContext;
        private readonly ITemplateRepository _templateRepository;

        public EmailService(ISettingRepository settingRepository, TDbContext dbContext, ITemplateRepository templateRepository)
        {
            _settingRepository = settingRepository;
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TemplateModel>();
            _templateRepository = templateRepository;
        }
    }
}
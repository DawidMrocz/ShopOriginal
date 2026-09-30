using Framework.Models.Template;

namespace Framework.Repositories.Template
{
    public interface ITemplateRepository
    {
        Task<TemplateModel?> Get(string templateName, int culture);
    }
}

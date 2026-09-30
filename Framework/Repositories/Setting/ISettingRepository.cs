using Framework.Models.Setting;

namespace Framework.Repositories.Setting
{
    public interface ISettingRepository
    {
        Task<SettingModel?> Get(string key);
    }
}

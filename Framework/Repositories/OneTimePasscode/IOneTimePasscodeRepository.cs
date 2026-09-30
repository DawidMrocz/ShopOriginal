using Framework.Models.OneTimePasscode;

namespace Framework.Repositories.OneTimePasscode
{

    public interface IOneTimePasscodeRepository<TUserId> : IGenericRepository<OneTimePasscodeModel<TUserId>, int>
    {
        Task<OneTimePasscodeModel<TUserId>?> GetByGuid(Guid guid);
    }
}

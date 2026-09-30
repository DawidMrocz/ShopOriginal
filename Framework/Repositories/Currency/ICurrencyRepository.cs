using Framework.Models.Currency;

namespace Framework.Repositories.Currency
{
    public interface ICurrencyRepository
    {
        Task<CurrencyModel> Get(int id);
        Task<CurrencyModel> GetByCode(string name);
    }
}

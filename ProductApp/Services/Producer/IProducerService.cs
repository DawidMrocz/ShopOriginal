using ProductApp.ApiModels.Producer;

namespace ProductApp.Services.Producer
{
    public interface IProducerService
    {
        Task<IEnumerable<object>> GetList();
        Task<object> Get(int producerId);
    }
}

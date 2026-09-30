using BackOfficeProductApp.ApiModels.Producer;

namespace BackOfficeProductApp.Services.Producer
{
    public interface IProducerService
    {
        Task<List<ProducerGetResponse>> GetList();
        Task<ProducerGetResponse> Get(int producerId);
        Task Create(ProducerCreateRequest request);
        Task Delete(int producerId);
    }
}

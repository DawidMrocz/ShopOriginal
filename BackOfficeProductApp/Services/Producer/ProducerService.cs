using BackOfficeProductApp.ApiModels.Producer;
using Framework.Attribiutes.Dependency;
using Framework.Repositories.File;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using ProductAppGrpc;

namespace BackOfficeProductApp.Services.Producer
{
    [DependencyInjection(typeof(IProducerService))]
    public class ProducerService : IProducerService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ProductAppGrpc.Producer.ProducerClient _client;

        public ProducerService(IFileRepository fileRepository, ProductAppGrpc.Producer.ProducerClient client)
        {
            _fileRepository = fileRepository;
            _client = client;
        }

        public async Task Create(ApiModels.Producer.ProducerCreateRequest request)
        {
            _client.Create(new ProductAppGrpc.ProducerCreateRequest()
            {
                Name = request.Name,
                LogoName = request.Logo.Name,
                Logo = ByteString.CopyFrom(_fileRepository.GetContent(request.Logo)),
            });
        }

        public async Task Delete(int producerId)
        {
            _client.Delete(new ProducerDeleteRequest()
            {
                ProducerId = producerId
            });
        }

        public async Task<ProducerGetResponse> Get(int producerId)
        {
            ProducerGetReply producer = _client.Get(new ProducerGetRequest()
            {
                ProducerId = producerId
            });

            return new ProducerGetResponse()
            {
                Id = producer.BaseRecords.Id,
                Name = producer.Name,
                Logo = producer.Logo.ToByteArray(),
            };
        }

        public async Task<List<ProducerGetResponse>> GetList()
        {
            ProducerGetListReply producers = _client.GetList(new Empty());

            return producers.Producers.Select(p => new ProducerGetResponse()
            {
                Id = p.BaseRecords.Id,
                Name = p.Name,
                Logo = p.Logo.ToByteArray(),
            }).ToList();
        }
    }
}

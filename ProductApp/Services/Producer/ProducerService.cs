using Contracts.RabbitMessages.Product;
using Framework.Attribiutes.Dependency;
using Framework.Models.File;
using Framework.Repositories.File;
using Google.Protobuf.WellKnownTypes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApp.ApiModels.Producer;
using ProductApp.ApiModels.Product;
using ProductApp.Data;
using ProductApp.Services.ProductService;
using ProductAppGrpc;

namespace ProductApp.Services.Producer
{
    [DependencyInjection(typeof(IProducerService))]
    public class ProducerService : IProducerService
    {
        private readonly ProductAppGrpc.Producer.ProducerClient _client;

        public ProducerService(ProductAppGrpc.Producer.ProducerClient client)
        {
            _client = client;
        }      

        public async Task<object> Get(int producerId)
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

        public async Task<IEnumerable<object>> GetList()
        {
            ProducerGetListReply producers = _client.GetList(new Empty());

            return producers.Producers.Select(p => new ProducerGetResponse()
            {
                Id = p.BaseRecords.Id,
                Name = p.Name,
                Logo= p.Logo.ToByteArray(),
            }).ToList() ;
        }
    }
}

using Framework.Models.File;
using Framework.Repositories.File;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductAppGrpc.Data;

namespace ProductAppGrpc.Services
{
    public class ProducerService : Producer.ProducerBase
    {
        private readonly ILogger<ProducerService> _logger;
        private readonly IFileRepository _fileRepository;

        private readonly ProductContext _productContext;

        public ProducerService(ILogger<ProducerService> logger, ProductContext productContext, IFileRepository fileRepository)
        {
            _logger = logger;
            _productContext = productContext;
            _fileRepository = fileRepository;
        }

        public override async Task<ProducerCreateReply> Create(ProducerCreateRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProducerModel model = new()
                {
                    Name = request.Name,
                };

                if (request.Logo is not null)
                {
                    FileModel fileModel = await _fileRepository.CreateRaw(request.Logo.ToByteArray(), request.LogoName);
                    model.Logo = fileModel;
                }

                await _productContext.Producers.AddAsync(model);

                await _productContext.SaveChangesAsync();

                return new ProducerCreateReply { ProducerId = model.Id };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<Empty> Delete(ProducerDeleteRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProducerModel model = await _productContext.Producers.FirstOrDefaultAsync(c => c.Id == request.ProducerId)
                        ?? throw new Exception("Producer not found");

                _productContext.Producers.Remove(model);
                await _productContext.SaveChangesAsync();

                return new Empty();
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<ProducerGetReply> Get(ProducerGetRequest request, ServerCallContext context)
        {
            try
            {
                Models.ProducerModel producer = await _productContext.Producers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.ProducerId)
                        ?? throw new Exception("Producer not found");

                return new ProducerGetReply()
                {
                    BaseRecords = new ProducerBaseRecord()
                    {
                        Id = producer.Id,
                        Created = Timestamp.FromDateTime(producer.Created.ToUniversalTime()),
                        CreatedById = producer.CreatedById,
                        Updated = producer.Updated is null ? null : Timestamp.FromDateTime(((DateTime)producer.Updated).ToUniversalTime()),
                        UpdatedById = producer.UpdatedById,
                    },
                    Name = producer.Name,
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }

        public override async Task<ProducerGetListReply> GetList(Empty request, ServerCallContext context)
        {
            try
            {
                List<Models.ProducerModel> producers = await _productContext.Producers.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

                ProducerGetListReply producersReponse = new();

                producersReponse.Producers.AddRange(producers.Select(c => new ProducerGetReply()
                {
                    BaseRecords = new ProducerBaseRecord()
                    {
                        Id = c.Id,
                        Created = Timestamp.FromDateTime(c.Created.ToUniversalTime()),
                        CreatedById = c.CreatedById,
                        Updated = c.Updated is null ? null : Timestamp.FromDateTime(((DateTime)c.Updated).ToUniversalTime()),
                        UpdatedById = c.UpdatedById,
                    },
                    Name = c.Name,
                }));

                return producersReponse;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.InnerException}"));
            }
        }
    }
}

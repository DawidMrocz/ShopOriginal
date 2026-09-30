using Framework.Models.File;

namespace BackOfficeProductApp.ApiModels.Producer
{
    public class ProducerGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public byte[] Logo { get; set; } = null!;
    }
}

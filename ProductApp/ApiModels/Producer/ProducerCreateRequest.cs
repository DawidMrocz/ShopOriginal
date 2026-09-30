namespace ProductApp.ApiModels.Producer
{
    public class ProducerCreateRequest
    {
        public string Name { get; set; } = null!;
        public IFormFile Logo { get; set; } = null!;
    }
}

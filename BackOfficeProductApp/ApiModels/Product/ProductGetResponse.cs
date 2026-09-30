using BackOfficeContracts.RabbitMessages;
using BackOfficeProductApp.ApiModels.Category;
using BackOfficeProductApp.ApiModels.Producer;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackOfficeProductApp.ApiModels.Product
{
    public class ProductGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public Price Price { get; set; } = null!;

        //FLAGS
        [NotMapped]
        public bool IsAvailable => Quantity > 0;

        //RELATIONS
        public ProducerGetResponse Producer { get; set; } = null!;
        public List<CategoryGetResponse> Categories { get; set; } = [];
        public byte[]? Photo { get; set; }
    }
}

using Contracts.RabbitMessages;
using ProductApp.Models;

namespace ProductApp.ApiModels.Product
{
    public class ProductGetListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public Price Price { get; set; } = null!;
        public bool IsFavourite { get; set; }
        public bool IsAvailable { get; set; }
        public int CountOfComments { get; set; }
        public double? OverallRate { get; set; }

        //RELATIONS
        public List<byte[]> Photos { get; set; } = [];
    }
}

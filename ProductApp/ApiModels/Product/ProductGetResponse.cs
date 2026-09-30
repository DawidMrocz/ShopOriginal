using Contracts.RabbitMessages;
using ProductApp.ApiModels.Category;
using ProductApp.ApiModels.Comment;
using ProductApp.ApiModels.Producer;
using ProductApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApp.ApiModels.Product
{
    public class ProductGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public Price Price { get; set; } = null!;
        public bool IsFavorite { get; set; }

        //FLAGS
        [NotMapped]
        public bool IsAvailable => Quantity > 0;
        [NotMapped]
        public int CountOfComments => Comments.Count;
        [NotMapped]
        public double? OverallRate => Comments.Select(c => c.Rate).Where(rate => rate is not null).Average();

        //RELATIONS
        public ProducerGetResponse Producer { get; set; } = null!;
        public List<CategoryGetResponse> Categories { get; set; } = [];
        public List<CommentGetResponse> Comments { get; set; } = [];
        public byte[]? Photo { get; set; }
    }
}

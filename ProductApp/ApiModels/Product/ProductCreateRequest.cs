using ProductApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.ApiModels.Product
{
    public class ProductCreateRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        //RELATIONS
        public int ProducerId { get; set; }
        public List<int> CategoriesId { get; set; } = [];
        [Required]
        public IFormFile? Photo { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BackOfficeProductApp.ApiModels.Product
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int ProducerId { get; set; }
        public List<int> CategoryIds { get; set; } = [];
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}

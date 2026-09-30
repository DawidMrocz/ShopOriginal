using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductAppGrpc.Models
{
    public class DiscountModel : BaseModel<int>
    {
        public decimal Percentage { get; set; }
        public string? Code { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public List<ProductModel> Products { get; set; } = new();
    }

    public class DiscountConfiguration : BaseModelConfiguration<DiscountModel, int>
    {
        public override void Configure(EntityTypeBuilder<DiscountModel> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}

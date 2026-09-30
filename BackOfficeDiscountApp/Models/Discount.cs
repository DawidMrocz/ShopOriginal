using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOfficeDiscountApp.Models
{
    public class Discount : BaseModel<int>
    {
        public decimal Percentage { get; set; }
        public string? Code { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public List<DiscoutProduct> Products { get; set; } = new();
    }

    public class DiscountConfiguration : BaseModelConfiguration<Discount, int>
    {
        public override void Configure(EntityTypeBuilder<Discount> modelBuilder)
        {
            base.Configure(modelBuilder);          
        }
    }

    public class DiscoutProduct : BaseModel<int>
    {
        public int DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
        public int ProductId { get; set; }
    }

    public class DiscoutProductConfiguration : BaseModelConfiguration<DiscoutProduct, int>
    {
        public override void Configure(EntityTypeBuilder<DiscoutProduct> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}

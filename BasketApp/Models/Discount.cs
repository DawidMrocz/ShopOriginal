using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Models
{
    public class Discount : BaseModel<int>
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public int DiscountId { get; set; }
        public bool Used { get; set; }

        public class DiscountConfiguration : BaseModelConfiguration<Discount, int>
        {
            public override void Configure(EntityTypeBuilder<Discount> modelBuilder)
            {
                base.Configure(modelBuilder);
            }
        }
    }
}

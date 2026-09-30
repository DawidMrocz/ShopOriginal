using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasketApp.Models
{
    public class BasketItem : BaseModel<int>
    {
        [NotMapped]
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        //RELATIONS
        public int BasketId { get; set; }
        public Basket Basket { get; set; } = null!;
        public int ProductId { get; set; }

        public class BasketItemConfiguration : BaseModelConfiguration<BasketItem, int>
        {
            public override void Configure(EntityTypeBuilder<BasketItem> modelBuilder)
            {
                base.Configure(modelBuilder);

                modelBuilder.Property(b => b.Quantity).IsRequired();
            }
        }
    }
}

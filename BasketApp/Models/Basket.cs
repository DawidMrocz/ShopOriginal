using Contracts.RabbitMessages;
using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasketApp.Models
{
    public class Basket : BaseModel<int>
    {
        [NotMapped]
        public int TotalQuantity => BasketItems.Count;
        [NotMapped]
        public decimal Price => BasketItems.Sum(p => p.Price);

        //FLAGS
        [NotMapped]
        public bool IsEmpty => TotalQuantity <= 0;

        //RELATIONS
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public List<BasketItem> BasketItems { get; set; } = [];

        public class BasketConfiguration : BaseModelConfiguration<Basket, int>
        {
            public override void Configure(EntityTypeBuilder<Basket> modelBuilder)
            {
                base.Configure(modelBuilder);

                modelBuilder
                    .HasOne(u => u.User)
                    .WithOne(c => c.Basket)
                    .HasForeignKey<Basket>(c => c.UserId);

                modelBuilder
                    .HasMany(u => u.BasketItems)
                    .WithOne(c => c.Basket)
                    .HasForeignKey(c => c.BasketId);
            }
        }
    }
}

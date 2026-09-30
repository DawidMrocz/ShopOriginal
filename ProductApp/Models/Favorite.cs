using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductApp.Models
{
    public class Favorite : ExternalBaseModel<int>
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public List<FavoriteItem> Items { get; set; } = [];
    }

    public class FavoriteConfiguration : ExternalBaseModelConfiguration<Favorite, int>
    {
        public override void Configure(EntityTypeBuilder<Favorite> modelBuilder)
        {
            base.Configure(modelBuilder);
            //modelBuilder
            //    .HasOne(c => c.Product)
            //    .WithMany(p => p.Favorites)
            //    .HasForeignKey(p => p.ProductId);
        }

    }
}

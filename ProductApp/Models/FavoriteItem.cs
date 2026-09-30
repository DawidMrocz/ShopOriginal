using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductApp.Models
{
    public class FavoriteItem : ExternalBaseModel<int>
    {
        public int FavoriteId { get; set; }
        public Favorite Favorite { get; set; } = null!;
        public int ProductId { get; set; }
    }

    public class FavoriteItemConfiguration : ExternalBaseModelConfiguration<FavoriteItem, int>
    {
        public override void Configure(EntityTypeBuilder<FavoriteItem> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}

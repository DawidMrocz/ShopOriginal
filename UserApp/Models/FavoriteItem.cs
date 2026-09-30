using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserApp.Models
{
    public class FavoriteItem : BaseModel<int>
    {
        public int FavoriteId { get; set; }
        public int ProductId { get; set; }
    }

    public class FavoriteItemConfiguration : BaseModelConfiguration<FavoriteItem, int>
    {
        public override void Configure(EntityTypeBuilder<FavoriteItem> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}

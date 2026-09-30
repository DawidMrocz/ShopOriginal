using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserApp.Models
{
    public class Favorite : BaseModel<int>
    {      
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public List<FavoriteItem> Items { get; set; } = [];
       
    }

    public class FavoriteConfiguration : BaseModelConfiguration<Favorite, int>
    {
        public override void Configure(EntityTypeBuilder<Favorite> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}

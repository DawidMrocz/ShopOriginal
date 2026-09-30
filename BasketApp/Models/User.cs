using Framework.Models.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Models
{
    public class User : ExternalUserBaseModel<Guid>
    {

        //RELATIONS
        public Basket Basket { get; set; } = null!;
        public List<Discount> Discounts { get; set; } = new();

        public class UserConfiguration : ExternalUserBaseModelConfiguration<User, Guid>
        {
            public override void Configure(EntityTypeBuilder<User> modelBuilder)
            {
                base.Configure(modelBuilder);
            }
        }
    }
}

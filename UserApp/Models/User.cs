using Framework.Models;
using Framework.Models.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserApp.Models
{
    public class User : UserBaseModel<Guid>  { }

    public class UserEntityTypeConfiguration : BaseModelConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}

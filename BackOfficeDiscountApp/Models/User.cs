using Framework.Models.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOfficeDiscountApp.Models
{
    public sealed class User : ExternalUserBaseModel<Guid>
    {
    }

    public class UserConfiguration : ExternalUserBaseModelConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Ignore(t => t.FirstName);
            modelBuilder.Ignore(t => t.LastName);
            modelBuilder.Ignore(t => t.Role);
            modelBuilder.Ignore(t => t.Photo);
            modelBuilder.Ignore(t => t.Street);
            modelBuilder.Ignore(t => t.Country);
            modelBuilder.Ignore(t => t.City);
            modelBuilder.Ignore(t => t.PostalCode);        
        }
    }
}

using Framework.Enums;
using Framework.Models.Token;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models.User
{
    public abstract class ExternalUserBaseModel<T> : UserBaseModel<T>
    {
        public T ExternalId { get; set; } = default!;
    }

    public abstract class ExternalUserBaseModelConfiguration<T, TId> : UserBaseModelConfiguration<T, TId> where T : ExternalUserBaseModel<TId>
    {
        public override void Configure(EntityTypeBuilder<T> modelBuilder)
        {
            modelBuilder.Property(e => e.ExternalId).IsRequired();
            modelBuilder.Ignore(t => t.PasswordSalt);
            modelBuilder.Ignore(t => t.PasswordHash);
            modelBuilder.Ignore(t => t.UnblockTime);
            modelBuilder.Ignore(t => t.Blocked);
            modelBuilder.Ignore(t => t.Activated);
            modelBuilder.Ignore(t => t.Tokens);
            modelBuilder.Ignore(t => t.Role);
        }
    }
}


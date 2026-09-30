using Framework.Models.Template;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models.Token
{
    public class TokenModel<TUserId> : BaseModel<int>
    {
        public string Value { get; set; } = null!;
        public DateTime ExpireDate { get; set; }
        public TUserId? UserId { get; set; }
    }

    public class TokenEntityTypeConfiguration<TUserId> : BaseModelConfiguration<TokenModel<TUserId>, int>
    {
        public override void Configure(EntityTypeBuilder<TokenModel<TUserId>> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.Property(p => p.Value).IsRequired();
            modelBuilder.Property(p => p.ExpireDate).IsRequired();
            modelBuilder.Property(p => p.UserId).IsRequired();
        }
    }
}

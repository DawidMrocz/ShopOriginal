using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models
{
    public abstract class ExternalBaseModel<T> : BaseModel<T>
    {
        public T ExternalId { get; set; } = default!;
    }

    public class ExternalBaseModelConfiguration<T, TId> : BaseModelConfiguration<T, TId> where T : ExternalBaseModel<TId>
    {
        public override void Configure(EntityTypeBuilder<T> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Property(e => e.ExternalId).IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models
{
    public abstract class BaseModel<TId>
    {
        public TId? Id { get; set; }
        public DateTime Created { get; set; }
        public int CreatedById { get; set; } = 1;
        public DateTime? Updated { get; set; }
        public int? UpdatedById { get; set; }
    }

    public abstract class BaseModelConfiguration<T, TId> : IEntityTypeConfiguration<T> where T : BaseModel<TId>
    {
        public virtual void Configure(EntityTypeBuilder<T> modelBuilder)
        {
            modelBuilder.Property(e => e.Created)
                .IsRequired()
                .HasDefaultValueSql("(getdate())");

            modelBuilder.Property(e => e.Updated)
                .HasDefaultValueSql("(getdate())");
        }
    }
}

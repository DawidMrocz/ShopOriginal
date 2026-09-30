using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductAppGrpc.Models
{
    public class CategoryModel : BaseModel<int>
    {
        public string Name { get; set; } = null!;
    }

    public class CategoryConfiguration : BaseModelConfiguration<CategoryModel, int>
    {
        public override void Configure(EntityTypeBuilder<CategoryModel> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();
        }

    }
}

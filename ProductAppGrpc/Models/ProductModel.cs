using Framework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAppGrpc.Models
{
    public class ProductModel : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        //FLAGS
        [NotMapped]
        public bool IsAvailable => Quantity > 0;

        //RELATIONS
        public int ProducerId { get; set; }
        public ProducerModel Producer { get; set; } = null!;
        public List<CategoryModel> Categories { get; set; } = [];
        public int Photo { get; set; }
    }

    public class ProductConfiguration : BaseModelConfiguration<ProductModel, int>
    {
        public override void Configure(EntityTypeBuilder<ProductModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder
                .HasOne(c => c.Producer)
                .WithOne();

            modelBuilder
                .HasMany(c => c.Categories)
                .WithMany();

            modelBuilder.Property(e => e.Name).IsRequired();
            modelBuilder.Property(e => e.Description).HasMaxLength(255);
            modelBuilder.Property(e => e.Quantity).IsRequired();
            modelBuilder.Property(e => e.Price).IsRequired();
        }
    }
}

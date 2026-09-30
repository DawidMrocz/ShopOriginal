using Contracts.RabbitMessages;
using Framework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserApp.Models
{
    public class Product : ExternalBaseModel<int>
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int ProducerId { get; set; }
        public int Photo { get; set; }
    }

    public class ProductConfiguration : ExternalBaseModelConfiguration<Product, int>
    {
        public override void Configure(EntityTypeBuilder<Product> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        }
    }
}

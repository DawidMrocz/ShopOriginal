using Framework.Models;
using Framework.Models.File;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserApp.Models
{
    public class Producer : ExternalBaseModel<int>
    {
        public string Name { get; set; } = null!;
        public int LogoId { get; set; }
    }

    public class ProducerConfiguration : ExternalBaseModelConfiguration<Producer, int>
    {
        public override void Configure(EntityTypeBuilder<Producer> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        }
    }
}

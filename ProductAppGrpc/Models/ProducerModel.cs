using Framework.Models;
using Framework.Models.File;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductAppGrpc.Models
{
    public class ProducerModel : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public int LogoId { get; set; }
        public FileModel Logo { get; set; } = null!;
    }

    public class ProducerConfiguration : BaseModelConfiguration<ProducerModel, int>
    {
        public override void Configure(EntityTypeBuilder<ProducerModel> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Property(e => e.Name).IsRequired();

            modelBuilder
                .HasOne(p => p.Logo)
                .WithOne().HasForeignKey<ProducerModel>(p => p.LogoId).IsRequired();
        }

    }
}

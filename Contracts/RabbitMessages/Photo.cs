using Framework.Models;
using Framework.Models.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contracts.RabbitMessages
{
    public class Photo : BaseModel<int>
    {
        public int ProductId { get; set; }
        public int FileId { get; set; }
        public FileModel File { get; set; } = null!;
    }

    public class PhotoConfiguration : BaseModelConfiguration<Photo, int>
    {
        public override void Configure(EntityTypeBuilder<Photo> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.HasOne(p => p.File).WithOne().HasForeignKey<Photo>(p => p.FileId).OnDelete(DeleteBehavior.Cascade).IsRequired();
        }
    }
}

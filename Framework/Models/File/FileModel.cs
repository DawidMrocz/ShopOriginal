using Framework.Enums;
using Framework.Models.Country;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models.File
{
    public class FileModel : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public byte[]? Content { get; set; }
        public Guid FileGuid { get; set; }
        public int? UserId { get; set; }
        public UserRole? Role { get; set; }
        public int? ExternalId { get; set; }
    }

    public class CountryEntityTypeConfiguration : BaseModelConfiguration<FileModel, int>
    {
        public override void Configure(EntityTypeBuilder<FileModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.Property(f => f.Name).IsRequired();
            modelBuilder.Property(f => f.Content).IsRequired(false);
            modelBuilder.Property(f => f.FileGuid).IsRequired();
            modelBuilder.Property(f => f.UserId).IsRequired(false);
            modelBuilder.Property(e => e.Role)
                .IsRequired(false)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v))
                .HasDefaultValue(UserRole.User);
            modelBuilder.Property(f => f.ExternalId).IsRequired(false);
        }
    }
}

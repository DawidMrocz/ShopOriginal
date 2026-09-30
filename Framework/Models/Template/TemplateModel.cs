using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Framework.Models.Country;
using Framework.Models.Culture;

namespace Framework.Models.Template
{
    public class TemplateModel : BaseModel<int>
    {
        public string Body { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string StrongName { get; set; } = null!;
        public string? Description { get; set; }
        public int CultureId { get; set; }
        public CultureModel Culture { get; set; } = null!;
    }

    public class TemplateEntityTypeConfiguration : BaseModelConfiguration<TemplateModel, int>
    {
        public override void Configure(EntityTypeBuilder<TemplateModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.Property(p => p.Body).IsRequired();
            modelBuilder.Property(p => p.Subject).HasMaxLength(500).IsRequired();
            modelBuilder.Property(p => p.Name).HasMaxLength(500).IsRequired();
            modelBuilder.Property(p => p.StrongName).HasMaxLength(250).IsRequired();
            modelBuilder.Property(p => p.Description).HasMaxLength(500).IsRequired(false);

            modelBuilder.HasOne(c => c.Culture).WithMany().HasForeignKey(c => c.CultureId);

            //modelBuilder.HasData(new List<TemplateModel>()
            //{
            //    new() {
            //        Id = 1,
            //        CreatedById = 1,
            //        Created = DateTime.Now,
            //        UpdatedById = 1,
            //        Updated = DateTime.Now,
            //        AreaCode = "+48",
            //        Code = "PL",
            //    },
            //    new() {
            //        Id = 2,
            //        CreatedById = 2,
            //        Created = DateTime.Now,
            //        UpdatedById = 2,
            //        Updated = DateTime.Now,
            //        AreaCode = "+44",
            //        Code = "GB",
            //    }
            //});
        }
    }
}

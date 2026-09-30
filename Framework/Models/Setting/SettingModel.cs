using Framework.Models.Label;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models.Setting
{
    public class SettingModel : BaseModel<int>
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class SettingEntityTypeConfiguration : BaseModelConfiguration<SettingModel, int>
    {
        public override void Configure(EntityTypeBuilder<SettingModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.Property(s => s.Key).IsRequired();
            modelBuilder.Property(s => s.Value).IsRequired();
            modelBuilder.Property(s => s.Description).IsRequired(false);

            //modelBuilder.HasData(new List<LabelModel>()
            //{
            //    new() {
            //        Id = 1,
            //        CreatedById = 1,
            //        Created = DateTime.Now,
            //        UpdatedById = 1,
            //        Updated = DateTime.Now,
            //        CultureId = 1,
            //        Key = "CountryName(PL)",
            //        Text = "Polska",
            //    },
            //    new() {
            //        Id = 2,
            //        CreatedById = 2,
            //        Created = DateTime.Now,
            //        UpdatedById = 2,
            //        Updated = DateTime.Now,
            //        CultureId = 1,
            //        Key = "CountryName(GB)",
            //        Text = "Wielka Brytania",
            //    },
            //    new() {
            //        Id = 3,
            //        CreatedById = 1,
            //        Created = DateTime.Now,
            //        UpdatedById = 1,
            //        Updated = DateTime.Now,
            //        CultureId = 2,
            //        Key = "CountryName(PL)",
            //        Text = "Poland",
            //    },
            //    new() {
            //        Id = 4,
            //        CreatedById = 2,
            //        Created = DateTime.Now,
            //        UpdatedById = 2,
            //        Updated = DateTime.Now,
            //        CultureId = 2,
            //        Key = "CountryName(GB)",
            //        Text = "Great Britam",
            //    }
            //});
        }
    }
}

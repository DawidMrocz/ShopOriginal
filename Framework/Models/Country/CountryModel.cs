using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models.Country
{
    public class CountryModel : BaseModel<int>
    {
        public string AreaCode { get; set; } = null!;
        public string Code { get; set; } = null!;
    }

    public class CountryEntityTypeConfiguration : BaseModelConfiguration<CountryModel, int>
    {
        public override void Configure(EntityTypeBuilder<CountryModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.HasData(new List<CountryModel>()
            {
                new() {
                    Id = 1,
                    CreatedById = 1,
                    Created = DateTime.Now,
                    UpdatedById = 1,
                    Updated = DateTime.Now,
                    AreaCode = "+48",
                    Code = "PL",
                },
                new() {
                    Id = 2,
                    CreatedById = 2,
                    Created = DateTime.Now,
                    UpdatedById = 2,
                    Updated = DateTime.Now,
                    AreaCode = "+44",
                    Code = "GB",
                }
            });
        }
    }
}

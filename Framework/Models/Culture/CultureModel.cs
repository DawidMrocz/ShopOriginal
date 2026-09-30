using Framework.Models.Currency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Models.Culture
{
    public class CultureModel : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public byte[]? Icon { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyModel Currency { get; set; } = null!;
    }

    public class CultureEntityTypeConfiguration : BaseModelConfiguration<CultureModel, int>
    {
        public override void Configure(EntityTypeBuilder<CultureModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.Property(p => p.Name).IsRequired();
            modelBuilder.Property(p => p.Description).IsRequired();
            modelBuilder.Property(p => p.Icon).IsRequired(false);
            modelBuilder.HasOne(c => c.Currency).WithMany().HasForeignKey(c => c.CurrencyId);

            modelBuilder.HasData(new List<CultureModel>()
            {
                new() {
                    Id = 1,
                    CreatedById = 1,
                    Created = DateTime.Now,
                    UpdatedById = 1,
                    Updated = DateTime.Now,
                    Name = "pl-PL",
                    Description = "Polski",
                    //Icon = 1,
                },
                new() {
                    Id = 2,
                    CreatedById = 2,
                    Created = DateTime.Now,
                    UpdatedById = 2,
                    Updated = DateTime.Now,
                    Name = "en-GB",
                    Description = "Angielski",
                    //Icon = 0.23m,
                }
            });
        }
    }
}

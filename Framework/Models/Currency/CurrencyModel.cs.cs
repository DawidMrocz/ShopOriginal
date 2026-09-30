using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Framework.Models.Currency
{
    public class CurrencyModel : BaseModel<int>
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public decimal Rate { get; set; }
    }
    public class CurrencyEntityTypeConfiguration : BaseModelConfiguration<CurrencyModel, int>
    {
        public override void Configure(EntityTypeBuilder<CurrencyModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.HasData(new List<CurrencyModel>()
            {
                new() {
                    Id = 1,
                    CreatedById = 1,
                    Created = DateTime.Now,
                    UpdatedById = 1,
                    Updated = DateTime.Now,
                    Name = "PLN",
                    Code = "PLN",
                    Rate = 1,
                },
                new() {
                    Id = 2,
                    CreatedById = 2,
                    Created = DateTime.Now,
                    UpdatedById = 2,
                    Updated = DateTime.Now,
                    Name = "EUR",
                    Code = "EUR",
                    Rate = 0.23m,
                }
            });
        }
    }
}

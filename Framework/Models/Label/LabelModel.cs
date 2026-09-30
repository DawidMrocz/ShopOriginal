using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Models.Label
{
    public class LabelModel : BaseModel<int>
    {
        [Required]
        public int CultureId { get; set; }
        [Required]
        public string Key { get; set; } = null!;
        [Required]
        public string Text { get; set; } = null!;
    }
    public class LabelEntityTypeConfiguration : BaseModelConfiguration<LabelModel, int>
    {
        public override void Configure(EntityTypeBuilder<LabelModel> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.HasData(new List<LabelModel>()
            {
                new() {
                    Id = 1,
                    CreatedById = 1,
                    Created = DateTime.Now,
                    UpdatedById = 1,
                    Updated = DateTime.Now,
                    CultureId = 1,
                    Key = "CountryName(PL)",
                    Text = "Polska",
                },
                new() {
                    Id = 2,
                    CreatedById = 2,
                    Created = DateTime.Now,
                    UpdatedById = 2,
                    Updated = DateTime.Now,
                    CultureId = 1,
                    Key = "CountryName(GB)",
                    Text = "Wielka Brytania",
                },
                new() {
                    Id = 3,
                    CreatedById = 1,
                    Created = DateTime.Now,
                    UpdatedById = 1,
                    Updated = DateTime.Now,
                    CultureId = 2,
                    Key = "CountryName(PL)",
                    Text = "Poland",
                },
                new() {
                    Id = 4,
                    CreatedById = 2,
                    Created = DateTime.Now,
                    UpdatedById = 2,
                    Updated = DateTime.Now,
                    CultureId = 2,
                    Key = "CountryName(GB)",
                    Text = "Great Britam",
                }
            });
        }
    }
}

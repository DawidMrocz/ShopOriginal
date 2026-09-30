using Framework.Enums;
using Framework.Models.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Framework.Models.User
{

    [Index(nameof(Email), IsUnique = true)]
    public abstract class UserBaseModel<T> : BaseModel<T>
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole Role { get; set; }
        [JsonIgnore]
        public byte[] PasswordHash { get; set; } = null!;
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; } = null!;
        public bool? Blocked { get; set; } = false;
        public bool Activated { get; set; } = false;
        public DateTime? UnblockTime { get; set; }
        public int? Photo { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public List<TokenModel<T>> Tokens { get; set; } = [];
    }

    public abstract class UserBaseModelConfiguration<T, TId> : BaseModelConfiguration<T, TId> where T : UserBaseModel<TId>
    { 
        public override void Configure(EntityTypeBuilder<T> modelBuilder)
        {
            base.Configure(modelBuilder);

            modelBuilder.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();
           
            modelBuilder.Property(e => e.Role)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v))
                .HasDefaultValue(UserRole.User);

            modelBuilder.Property(e => e.Street).HasMaxLength(255);
            modelBuilder.Property(e => e.City).HasMaxLength(255);
            modelBuilder.Property(e => e.Country).HasMaxLength(255);
            modelBuilder.Property(e => e.PostalCode).HasMaxLength(255);
        }
    }
}

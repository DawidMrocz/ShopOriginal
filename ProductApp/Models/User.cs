using Framework.Enums;
using Framework.Models.File;
using Framework.Models;
using Framework.Models.Token;
using Framework.Models.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductApp.Models
{
    public sealed class User : ExternalUserBaseModel<Guid>
    {
        //RELATIONS
        public List<Comment> Comments { get; set; } = [];
        public Favorite Favorites { get; set; } = null!;
    }

    public class UserConfiguration : ExternalUserBaseModelConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Ignore(t => t.FirstName);
            modelBuilder.Ignore(t => t.LastName);
            modelBuilder.Ignore(t => t.Role);
            modelBuilder.Ignore(t => t.Photo);
            modelBuilder.Ignore(t => t.Street);
            modelBuilder.Ignore(t => t.Country);
            modelBuilder.Ignore(t => t.City);
            modelBuilder.Ignore(t => t.PostalCode);

            modelBuilder
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            //modelBuilder
            //    .HasOne(u => u.Favorites)
            //    .WithOne(c => c.User)
            //    .HasForeignKey(c => c.UserId);         
        }
    }
}

//public class Category : BaseModel<int>
//{
//    public string Name { get; set; } = null!;
//}

//public class CategoryConfiguration : BaseModelConfiguration<Category, int>
//{
//    public override void Configure(EntityTypeBuilder<Category> modelBuilder)
//    {
//        base.Configure(modelBuilder);
//        modelBuilder.Property(e => e.Name)
//            .HasMaxLength(50)
//            .IsRequired();
//    }

//}

//public class Producer : BaseModel<int>
//{
//    public string Name { get; set; } = null!;
//    public int LogoId { get; set; }
//    public FileModel Logo { get; set; } = null!;
//}

//public class ProducerConfiguration : BaseModelConfiguration<Producer, int>
//{
//    public override void Configure(EntityTypeBuilder<Producer> modelBuilder)
//    {
//        base.Configure(modelBuilder);
//        modelBuilder.Property(e => e.Name).IsRequired();

//        modelBuilder
//            .HasOne(p => p.Logo)
//            .WithOne().HasForeignKey<Producer>(p => p.LogoId).IsRequired();
//    }

//}


//public class Product : BaseModel<int>
//{
//    public string Name { get; set; } = null!;
//    public string? Description { get; set; }
//    public int Quantity { get; set; }
//    public decimal Price { get; set; }

//    //FLAGS
//    [NotMapped]
//    public bool IsAvailable => Quantity > 0;
//    [NotMapped]
//    public int CountOfComments => Comments.Count;
//    [NotMapped]
//    public double? OverallRate => Comments.Select(c => c.Rate).Where(rate => rate is not null).Average();

//    //RELATIONS
//    public int ProducerId { get; set; }
//    public Producer Producer { get; set; } = null!;
//    public List<Category> Categories { get; set; } = [];
//    public List<Comment> Comments { get; set; } = [];
//    public int Photo { get; set; }
//    public List<FavoriteItem> FavoriteItems { get; set; } = [];
//}

//public class ProductConfiguration : BaseModelConfiguration<Product, int>
//{
//    public override void Configure(EntityTypeBuilder<Product> modelBuilder)
//    {
//        base.Configure(modelBuilder);

//        modelBuilder
//            .HasOne(c => c.Producer)
//            .WithOne();

//        modelBuilder
//            .HasMany(c => c.Categories)
//            .WithMany();

//        modelBuilder
//           .HasMany(c => c.Comments)
//           .WithOne(p => p.Product)
//           .HasForeignKey(p => p.ProductId);

//        modelBuilder.Property(e => e.Name).IsRequired();
//        modelBuilder.Property(e => e.Description).HasMaxLength(255);
//        modelBuilder.Property(e => e.Quantity).IsRequired();
//        modelBuilder.Property(e => e.Price).IsRequired();
//    }

//}
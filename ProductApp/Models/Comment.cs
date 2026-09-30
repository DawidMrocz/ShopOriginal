using Framework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductApp.Models
{
    public class Comment : BaseModel<int>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public long? Rate { get; set; }


        //RELATIONS
        public int ProductId { get; set; }
        //public Product Product { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public class CommentConfiguration : BaseModelConfiguration<Comment, int>
    {
        public override void Configure(EntityTypeBuilder<Comment> modelBuilder)
        {
            base.Configure(modelBuilder);
            modelBuilder.Property(e => e.Title).IsRequired();
            modelBuilder.Property(e => e.Description).IsRequired();
            modelBuilder.Property(e => e.Rate).IsRequired(false);
        }

    }
}

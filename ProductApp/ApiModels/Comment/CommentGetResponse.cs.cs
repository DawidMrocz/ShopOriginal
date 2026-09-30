using Framework.Models;

namespace ProductApp.ApiModels.Comment
{
    public class CommentGetResponse : BaseModel<int>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public long? Rate { get; set; }
        public string Email { get; set; } = null!;
        public string? Login { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        //public Files Files { get; set; } = new();
        //public Address? Address { get; set; }

    }
}

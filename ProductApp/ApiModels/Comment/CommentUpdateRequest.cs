using System.ComponentModel.DataAnnotations;

namespace ProductApp.ApiModels.Comment
{
    public class CommentUpdateRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        [Range(1, 5)]
        public long? Rate { get; set; }
    }
}

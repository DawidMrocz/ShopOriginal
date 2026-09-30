using ProductApp.ApiModels.Comment;

namespace ProductApp.Services.Comment
{
    public interface ICommentService
    {
        Task Create(CommentCreateRequest request, int productId, Guid userId);
        //Task<List<CommentGetResponse>> GetList(int productId);
        Task Delete(int commentId, Guid userId);
        Task Update(CommentUpdateRequest request, int commentId, Guid userId);
    }
}

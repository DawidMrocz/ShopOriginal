using Framework.Attribiutes.Dependency;
using Microsoft.EntityFrameworkCore;
using ProductApp.ApiModels.Comment;
using ProductApp.Data;
using ProductApp.Models;

namespace ProductApp.Services.Comment
{
    [DependencyInjection(typeof(ICommentService))]
    public class CommentService : ICommentService
    {
        private readonly ProductContext _productContext;

        public CommentService(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task Create(CommentCreateRequest request, int productId, Guid userId)
        {
            User user = await _productContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId)
                ?? throw new Exception("User not found");

            Models.Comment model = new()
            {
                Created = DateTime.Now,
                Title = request.Title,
                Description = request.Description,
                Rate = request.Rate,
                UserId = user.Id,
                ProductId = productId,
            };

            await _productContext.AddAsync(model);
            await _productContext.SaveChangesAsync();
        }

        public async Task Delete(int commentId, Guid userId)
        {
            User user = await _productContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId)
                ?? throw new Exception("User not found");

            Models.Comment model = await _productContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId)
                ?? throw new Exception("Comment not found");

            if (model.UserId != user.Id) throw new Exception("You cannot modify somebody's comment");

            _productContext.Comments.Remove(model);
            await _productContext.SaveChangesAsync();
        }

        public async Task<List<CommentGetResponse>> GetList(int productId)
        {
            return await _productContext.Comments
                .Where(c => c.ProductId == productId)
                .Include(u => u.User)
                .AsNoTracking()
                .OrderBy(c => c.Created)
                .Select(c => new CommentGetResponse()
                {
                    Id = c.Id,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Title = c.Title,
                    Description = c.Description,
                    Rate = c.Rate,
                    Email = c.User.Email,
                    LastName = c.User.LastName,
                    FirstName = c.User.FirstName,
                })
                .ToListAsync();
        }

        public async Task Update(CommentUpdateRequest request, int commentId, Guid userId)
        {
            User user = await _productContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId)
               ?? throw new Exception("User not found");

            Models.Comment model = await _productContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId)
                ?? throw new Exception("Comment not found");

            if (model.UserId != user.Id) throw new Exception("You cannot modify somebody's comment");

            model.Updated = DateTime.Now;
            model.Title = request.Title;
            model.Description = request.Description;
            model.Rate = request.Rate;

            await _productContext.SaveChangesAsync();
        }
    }
}

using Framework.Repositories.File;
using Framework.Repositories.User;
using Framework.Services.User;
using UserApp.Data;
using UserApp.Services.Favorite;

namespace UserApp.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        UserRepository<UserContext, Models.User, Guid> UserRepository { get; }
        FileRepository<UserContext> FileRepository { get; }
        UserService<Models.User, Guid> UserService { get; }
        FavoriteService FavoriteService { get; }
        Task Save();
    }
}
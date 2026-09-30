using UserApp.ApiModels.Favorite;

namespace UserApp.Services.Favorite
{
    public interface IFavoriteService
    {
        Task<FavoriteResponse> Get(Guid userId, string culture);
        Task Delete(int favoriteItemId, Guid userId);
    }
}

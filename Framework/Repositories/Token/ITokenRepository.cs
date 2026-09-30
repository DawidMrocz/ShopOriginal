using Framework.Models.Token;

namespace Framework.Repositories.Token
{
    public interface ITokenRepository<TUserId>
    {
        Task<TokenModel<TUserId>> GetTokenByValue(string value);
        Task DeleteToken(int id);
        Task CreateToken(TokenModel<TUserId> refreshToken);
    }
}

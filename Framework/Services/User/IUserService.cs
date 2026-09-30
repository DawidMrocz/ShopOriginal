using Framework.ApiModels.Access.Request;
using Framework.ApiModels.User;
using Framework.Enums;
using Framework.Models.User;
using Microsoft.AspNetCore.Http;

namespace Framework.Services.User
{
    public interface IUserService<TEntity, TUserId> where TEntity : UserBaseModel<TUserId>
    {
        Task<TUserId> Create<TRequest>(TRequest request) where TRequest : ICreateRequest;
        Task Update<TRequest>(TUserId id, TRequest request) where TRequest : IUpdateRequest;
        Task Activate(string activateCode);
        Task ChangeRole(TUserId id, UserRole role);
        Task ChangePassword(TUserId id, ChangePasswordRequest request);
        Task Delete(TUserId id);
        Task RemindPassword(UserForgotPassword request, string culture);
        Task Unblock(TUserId id);
        Task Block(TUserId id, DateTime? blockTo);
        Task AddProfilePhoto(IFormFile file, TUserId id);
        Task DeleteProfilePhoto(TUserId id);
    }
}
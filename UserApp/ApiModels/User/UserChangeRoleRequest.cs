using Framework.Enums;

namespace UserApp.ApiModels.User
{
    public class UserChangeRoleRequest
    {
        public Guid Id { get; set; }
        public UserRole Role { get; set; }
    }
}

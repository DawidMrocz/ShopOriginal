using Framework.Enums;

namespace BackOfficeUserApp.ApiModels.User
{
    public class UserChangeRoleRequest
    {
        public Guid Id { get; set; }
        public UserRole Role { get; set; }
    }
}

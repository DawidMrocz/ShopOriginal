using Framework.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Framework.Middlewares
{
    public class AutoryzacjaAttribute : AuthorizeAttribute, IAuthorizationRequirement,
                                     IAuthorizationRequirementData
    {
        public UserRole? Role { get; }
        public AutoryzacjaAttribute() { }
        public AutoryzacjaAttribute(UserRole role) => Role = role;

        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}

namespace Framework.Models.Authentication
{
    public class AuthenticationUser : IAuthenticationUser
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string AuthorizeToken { get; set; } = null!;
        public string Culture { get; set; } = null!;
    }
}

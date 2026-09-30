namespace Framework.Models.Authentication
{
    public interface IAuthenticationUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string AuthorizeToken { get; set; }
        public string Culture { get; set; }
    }
}

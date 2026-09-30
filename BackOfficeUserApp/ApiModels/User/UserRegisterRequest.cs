using Framework.Services.User;

namespace BackOfficeUserApp.ApiModels.User
{
    public class UserRegisterRequest : ICreateRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

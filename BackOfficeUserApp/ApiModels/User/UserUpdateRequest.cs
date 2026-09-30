using Framework.Services.User;

namespace BackOfficeUserApp.ApiModels.User
{
    public class UserUpdateRequest : IUpdateRequest
    {
        public IFormFile? Photo { get; set; }
        public List<IFormFile>? Documents { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

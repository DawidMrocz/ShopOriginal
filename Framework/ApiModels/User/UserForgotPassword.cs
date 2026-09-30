using System.ComponentModel.DataAnnotations;

namespace Framework.ApiModels.User
{
    public class UserForgotPassword
    {
        [Required]
        [EmailAddress(ErrorMessage = "Nie poprawny format adresu e-mail")]
        public string Email { get; set; } = null!;
    }
}

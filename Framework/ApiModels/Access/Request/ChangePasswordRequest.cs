using Framework.Attribiutes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Framework.ApiModels.Access.Request
{
    public class ChangePasswordRequest : IValidatableObject
    {
        [Required]
        [PasswordValidation]
        public string NewPassword { get; set; } = null!;
        [Required]
        [PasswordValidation]
        public string RepeatPassword { get; set; } = null!;
        [Required]
        [PasswordValidation]
        public string OldPassword { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NewPassword.Equals(RepeatPassword))
            {
                yield return new ValidationResult(
                    $"Passwords are not the same",
                    new[] { nameof(RepeatPassword) });
            }
        }
    }
}

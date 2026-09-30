using AngleSharp.Text;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Framework.Attribiutes.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        public PasswordValidationAttribute() { }
        protected override ValidationResult? IsValid(object? value,
                                             ValidationContext validationContext)
        {
            IConfiguration? configuration = validationContext.GetService(typeof(IConfiguration)) as IConfiguration
                ?? throw new Exception("Service not found");

            int paswordLenght = int.Parse(configuration["IdentityOptions:Password:Lenght"] ?? throw new Exception("Appsetting is required"));
            bool paswordContainNumbers = bool.Parse(configuration["IdentityOptions:Password:ContainNumbers"] ?? throw new Exception("Appsetting is required"));
            bool paswordContainSigns = bool.Parse(configuration["IdentityOptions:Password:ContainSpecialSigns"] ?? throw new Exception("Appsetting is required"));
            bool paswordContainBigLetters = bool.Parse(configuration["IdentityOptions:Password:ContainBigLetters"] ?? throw new Exception("Appsetting is required"));
            bool paswordContainSmallLetters = bool.Parse(configuration["IdentityOptions:Password:ContainSmallLetters"] ?? throw new Exception("Appsetting is required"));

            if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult("Password is required.");

            string passwordValue = value.ToString()!;

            if (passwordValue.Length < paswordLenght) return new ValidationResult($"Password tmust have at least {paswordLenght} length");

            if (paswordContainNumbers)
                if (!new Regex(@"\d").IsMatch(passwordValue)) return new ValidationResult("Password must contain numbers");

            if (paswordContainBigLetters)
                if (!new Regex(@"[A-Z]").IsMatch(passwordValue)) return new ValidationResult("Password must contain big letters");

            if (paswordContainSmallLetters)
                if (!new Regex(@"[a-z]").IsMatch(passwordValue)) return new ValidationResult("Password must contain small letters");

            if (paswordContainSigns)
                if (passwordValue.Any(
                    character => !character.IsLetter() &&
                    !character.IsDigit() &&
                    !character.IsWhiteSpaceCharacter()
                    )) return new ValidationResult("Password msut contain specjal signs");

            return ValidationResult.Success;
        }
    }
}

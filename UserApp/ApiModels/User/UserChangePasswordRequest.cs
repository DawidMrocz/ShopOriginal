namespace UserApp.ApiModels.User
{
    public class UserChangePasswordRequest
    {
        public string Password { get; set; } = null!;
        public string RepeatPassword { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
    }

    public interface IPasswordChange
    {
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string OldPassword { get; set; }
    }
}

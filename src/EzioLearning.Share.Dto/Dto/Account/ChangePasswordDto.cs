namespace EzioLearning.Share.Dto.Account
{
    public class ChangePasswordDto
    {
        public string Password { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

namespace EzioLearning.Core.Dto.Account
{
    public class ChangeEmailConfirmApiDto
    {
        public string VerifyCode { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

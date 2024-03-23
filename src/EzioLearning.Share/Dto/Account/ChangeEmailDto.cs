namespace EzioLearning.Share.Dto.Account
{
    public class ChangeEmailDto
    {
        public string CurrentEmail { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;
        public string ClientUrl { get; set; } = string.Empty;

    }
}

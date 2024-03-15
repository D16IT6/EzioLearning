namespace EzioLearning.Core.Dto.Account
{
    public class ChangeEmailApiDto
    {
        public string CurrentEmail { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;

        public string ClientUrl { get; set; } = string.Empty;
    }
}

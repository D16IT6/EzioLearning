namespace EzioLearning.Core.Dtos.Auth
{
    public class RequestNewTokenDto
    {
        public required string UserName { get; set; }
        public required string RefreshToken { get; set; }
    }
}

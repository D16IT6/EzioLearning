namespace EzioLearning.Core.Dtos.Auth
{
    public class RequestNewTokenDto
    {
        public required string UserName { get; init; }
        public required string RefreshToken { get; init; }
    }
}

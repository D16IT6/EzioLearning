namespace EzioLearning.Api.Models.Token
{
    public class JwtResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}

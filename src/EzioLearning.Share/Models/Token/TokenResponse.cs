namespace EzioLearning.Share.Models.Token;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool Compare(TokenResponse other)
    {
        return string.IsNullOrWhiteSpace(AccessToken) && string.IsNullOrWhiteSpace(RefreshToken) &&
               AccessToken.Equals(other.AccessToken) && RefreshToken.Equals(other.RefreshToken);
    }

}
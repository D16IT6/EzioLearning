namespace EzioLearning.Api.Models.Auth
{
    public class Authentication
    {
        public Google Google { get; set; } = new();
        public Facebook Facebook { get; set; } = new();
    }

    public class Google
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    } public class Facebook
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}

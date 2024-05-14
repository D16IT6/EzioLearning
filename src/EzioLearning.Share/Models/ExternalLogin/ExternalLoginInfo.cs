
using EzioLearning.Share.Models.Token;

namespace EzioLearning.Share.Models.ExternalLogin
{
    public class ExternalLoginCacheInfo
    {
        public bool BackToLogin { get; set; }

        public TokenResponse Token { get; set; } = new();
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string LoginProvider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public bool NeedRegister { get; set; }

    }
}

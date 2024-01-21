
namespace EzioLearning.Core.Models.Auth
{
    public class AuthenticateRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EzioLearning.Api.Filters
{
    public class GoogleJwtTokenHandler(string clientId) : TokenHandler
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        

        public override SecurityToken ReadToken(string token)
        {
            return _tokenHandler.ReadToken(token);
        }

        public bool CanValidateToken => true;

        public override int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public override async Task<TokenValidationResult> ValidateTokenAsync(string token, TokenValidationParameters validationParameters)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                    token, new GoogleJsonWebSignature.ValidationSettings() { Audience = new[] { clientId }});

            try
            {
                //    var principle = new ClaimsPrincipal();
                //principle.AddIdentity(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
                var validatedToken = new TokenValidationResult()
                {
                    IsValid = true,
                    Claims =
                     {
                         new (ClaimTypes.NameIdentifier, payload.Name),
                         new (ClaimTypes.Name, payload.Name),
                         new (JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                         new (JwtRegisteredClaimNames.GivenName, payload.GivenName),
                         new (JwtRegisteredClaimNames.Email, payload.Email),
                         new (JwtRegisteredClaimNames.Sub, payload.Subject),
                         new (JwtRegisteredClaimNames.Iss, payload.Issuer)
                     },
                    TokenType = JwtBearerDefaults.AuthenticationScheme

                };
                return validatedToken;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new TokenValidationResult()
                {
                    IsValid = false,
                    Claims = {}
                };

            }
        }
    }
}

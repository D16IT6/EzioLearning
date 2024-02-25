using Blazored.LocalStorage;
using EzioLearning.Core.Models.Token;
using EzioLearning.Wasm.Providers;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EzioLearning.Core.Models.Response;

namespace EzioLearning.Wasm.Services
{
    public class TokenService(ILocalStorageService localStorageService) : ITokenService
    {
        public async Task SaveFromResponse(ResponseBase response)
        {
            if (response is ResponseBaseWithData<TokenResponse> responseWithData)
            {
                await SaveToken(responseWithData.Data!);
            }
        }

        public async Task SaveToken(TokenResponse token)
        {
            if (!string.IsNullOrEmpty(token.AccessToken) &&
                !string.IsNullOrEmpty(token.RefreshToken))
            {
                await localStorageService.SetItemAsStringAsync(LocalStorageConstants.AccessToken, token.AccessToken);
                await localStorageService.SetItemAsStringAsync(LocalStorageConstants.RefreshToken, token.RefreshToken);
            }
        }

        public async Task DeleteToken()
        {
            await localStorageService.RemoveItemAsync(LocalStorageConstants.AccessToken);
            await localStorageService.RemoveItemAsync(LocalStorageConstants.RefreshToken);
        }

        public async Task<IEnumerable<Claim>> ParseClaimsFromJwt(string? accessToken)
        {
            accessToken ??= await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

            var jwtTokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

            return jwtTokenHandler.Claims;
        }

        public async Task<TokenResponse> GetTokenFromLocalStorage()
        {
            var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);
            var refreshToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.RefreshToken);
            return new TokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<bool> IsTokenExpired(string? accessToken)
        {
            accessToken ??= await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

            var jwtHandler = new JwtSecurityTokenHandler();

            if (jwtHandler.CanReadToken(accessToken))
            {
                var jwtToken = jwtHandler.ReadJwtToken(accessToken);
                var expiration = jwtToken.ValidTo;
                return expiration < DateTime.UtcNow;
            }

            return true;
        }
    }
}

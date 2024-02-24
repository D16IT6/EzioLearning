using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

using System.Security.Claims;
using System.Text.Json;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Net;
using EzioLearning.Blazor.Client.Providers;
using EzioLearning.Core.Dtos.Auth;

namespace EzioLearning.Wasm.Providers
{
	public class ApiAuthenticationStateProvider(ILocalStorageService localStorageService, HttpClient httpClient) : AuthenticationStateProvider
	{
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var emptyAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			var accessToken = await localStorageService.GetItemAsync<string>(LocalStorageConstants.AccessToken) ?? string.Empty;

			if (string.IsNullOrWhiteSpace(accessToken))
			{
				return emptyAuthenticationState;
			}

			if (IsTokenExpired(accessToken))
			{
				var refreshToken = await localStorageService.GetItemAsync<string>(LocalStorageConstants.RefreshToken) ?? string.Empty;

				if (await GenerateNewToken(accessToken, refreshToken)
					is not ResponseBaseWithData<TokenResponse> response)
				{
					return emptyAuthenticationState;
				}

				accessToken = response.Data!.AccessToken ?? string.Empty;
			}
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(accessToken), ApiConstants.ApiAuthenticationType)));
		}


		public async Task<ResponseBase?> GenerateNewToken(string accessToken, string refreshToken)
		{
			var userName = ParseClaimsFromJwt(accessToken)
				.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!
				.Value;

			if (string.IsNullOrEmpty(userName)) return null;


			var requestModel = new RequestNewTokenDto()
			{
				UserName = userName,
				RefreshToken = refreshToken
			};

			ResponseBase? data = null;
			var response = await httpClient.PostAsJsonAsync("api/Auth/RefreshToken", requestModel);
			var stream = await response.Content.ReadAsStreamAsync();

			switch (response.StatusCode)
			{
				case HttpStatusCode.BadRequest:

					data = await JsonSerializer.DeserializeAsync<ResponseBase>(stream, JsonCommonOptions.DefaultSerializer);
					break;
				case HttpStatusCode.OK:

					data = await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream, JsonCommonOptions.DefaultSerializer);

					var dataChild = data as ResponseBaseWithData<TokenResponse>;

					await localStorageService.SetItemAsync(LocalStorageConstants.AccessToken, dataChild?.Data?.AccessToken);
					await localStorageService.SetItemAsync(LocalStorageConstants.RefreshToken, dataChild?.Data?.RefreshToken);

					return dataChild;
			}

			return data;
		}

		private bool IsTokenExpired(string token)
		{
			var jwtHandler = new JwtSecurityTokenHandler();

			if (jwtHandler.CanReadToken(token))
			{
				var jwtToken = jwtHandler.ReadJwtToken(token);
				var expiration = jwtToken.ValidTo;
				return expiration < DateTime.UtcNow;
			}

			return true;
		}

		public void MarkUserAsAuthenticated(string userName)
		{
			var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name,
						userName)
				},
				ApiConstants.ApiAuthenticationType));
			var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
			NotifyAuthenticationStateChanged(authState);
		}

		public void MarkUserAsLoggedOut()
		{
			var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
			var authState = Task.FromResult(new AuthenticationState(anonymousUser));
			NotifyAuthenticationStateChanged(authState);
		}

		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
	var jwtTokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

			return jwtTokenHandler.Claims;
		}


		private byte[] ParseBase64WithoutPadding(string base64)
		{
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}

	}
}

using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace EzioLearning.Wasm.Hubs
{
    public class HubConnectionManager(ITokenService tokenService, AuthenticationStateProvider authenticationStateProvider)
    {
        private static readonly Dictionary<HubConnectionEndpoints, HubConnection> HubConnections = new();

        public async Task<HubConnection?> GetHubConnectionAsync(HubConnectionEndpoints hubConnectionEndpoint, bool needAuthenticate)
        {
            if (needAuthenticate)
            {
                var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();

                if (authenticationState.User.Identity is not { IsAuthenticated: true }) return null;
            }

            var hubUrl = $"{ApiConstants.BaseUrl}{Enum.GetName(hubConnectionEndpoint)}Hub";

            if (HubConnections.TryGetValue(hubConnectionEndpoint, out var hubConnection)) return hubConnection;

            var accessToken = (await tokenService.GetTokenFromLocalStorage()).AccessToken;

            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(accessToken)!;
                })
                .AddJsonProtocol()
                
                .Build();

            HubConnections[hubConnectionEndpoint] = hubConnection;

            return HubConnections[hubConnectionEndpoint];

        }
    }
}

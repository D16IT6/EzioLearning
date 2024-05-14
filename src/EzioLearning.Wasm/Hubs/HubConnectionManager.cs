using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace EzioLearning.Wasm.Hubs
{
    public class HubConnectionManager(ITokenService tokenService)
    {
        private static readonly Dictionary<string, HubConnection> HubConnections = new();

        public async Task<HubConnection> GetHubConnectionAsync(string hubUrl)
        {
            if (!hubUrl.EndsWith("Hub")) hubUrl += "Hub";

            if (HubConnections.TryGetValue(hubUrl, out var hubConnection)) return hubConnection;

            var accessToken = (await tokenService.GetTokenFromLocalStorage()).AccessToken;
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Path.Combine(ApiConstants.BaseUrl, hubUrl), options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(accessToken);
                })
                .AddMessagePackProtocol()
                .Build();

            await hubConnection.StartAsync();
            HubConnections[hubUrl] = hubConnection;

            return HubConnections[hubUrl];
        }
    }
}

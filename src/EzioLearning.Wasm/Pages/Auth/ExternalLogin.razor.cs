﻿using Blazored.LocalStorage;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Providers;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Pages.Auth
{
    public partial class ExternalLogin
    {
        [SupplyParameterFromQuery]
        public bool BackToLogin { get; set; }

        [SupplyParameterFromQuery]
        public bool NeedRegister { get; set; }

        [SupplyParameterFromQuery]
        public Guid? UserId { get; set; }
        [SupplyParameterFromQuery]
        public string? Email { get; set; }
        [SupplyParameterFromQuery]
        public string? FirstName { get; set; }
        [SupplyParameterFromQuery]
        public string? LastName { get; set; }

        [SupplyParameterFromQuery] private string[] Errors { get; set; } = [];

        [SupplyParameterFromQuery]
        public string? AccessToken { get; set; }
        [SupplyParameterFromQuery]
        public string? RefreshToken { get; set; }

        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILogger<ExternalLogin> Logger { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {

            if (!string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken))
            {
                await LocalStorageService.SetItemAsStringAsync(LocalStorageConstants.AccessToken, AccessToken);
                await LocalStorageService.SetItemAsStringAsync(LocalStorageConstants.RefreshToken, RefreshToken);

                NavigationManager.NavigateTo(RouteConstants.Home);
            }

            if (BackToLogin)
            {
                NavigationManager.NavigateTo(RouteConstants.Login);
            }

            if (NeedRegister)
            {
                var queryStringParams = new Dictionary<string, object?>()
                {
                    {nameof(Email),Email},
                    {nameof(FirstName),FirstName},
                    {nameof(LastName),LastName},
                    {"FromExternal",true}
                };
                var registerUrl = NavigationManager.GetUriWithQueryParameters(RouteConstants.Register, queryStringParams);

                Logger.LogInformation(registerUrl);

                NavigationManager.NavigateTo(registerUrl);
            }

        }
    }
}
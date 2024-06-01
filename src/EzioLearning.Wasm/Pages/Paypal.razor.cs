using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Pages
{
    public partial class Paypal
    {

        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        private IJSObjectReference JsObject { get; set; } =default!;
        private IJSObjectReference PaypalJsObject { get; set; } =default!;

        protected override async Task OnInitializedAsync()
        {
            JsObject = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/Paypal.razor.js");

            await JsObject.InvokeVoidAsync("paypalStyle", ApiConstants.BaseUrl + "api/Payment/");

        }
    }
}

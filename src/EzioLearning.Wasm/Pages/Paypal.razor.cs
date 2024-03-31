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
            //var clientId = "AfADgP3zJGjwnAl3m0dK__-4gx4KLeebAnwOJfylRR6ACUr8RNwPX0gF4yl7g0JDAKxHq_c15JOSAsV3";
            //PaypalJsObject= await JsRuntime.InvokeAsync<IJSObjectReference>("import",
            //    $"https://www.paypal.com/sdk/js?client-id={clientId}&components=buttons");

            JsObject = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/Paypal.razor.js");

            await JsObject.InvokeVoidAsync("paypalStyle", ApiConstants.BaseUrl + "api/Payment/");

        }
    }
}

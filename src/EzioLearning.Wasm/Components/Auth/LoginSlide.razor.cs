using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Components.Auth
{
    public partial class LoginSlide
    {
        [Inject] private IJSRuntime JsRunTime { get; set; } = default!;
        private IJSObjectReference JsObjectReference { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JsObjectReference =
                    await JsRunTime.InvokeAsync<IJSObjectReference>
                        ("import", $"/Components/Auth/{nameof(LoginSlide)}.razor.js");
            }

            await JsObjectReference.InvokeVoidAsync("loadComponents");

        }
    }
}

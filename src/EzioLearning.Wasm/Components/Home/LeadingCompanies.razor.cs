﻿using BlazorAnimate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Components.Home;

public partial class LeadingCompanies
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }

    [Inject] private IJSRuntime JsRunTime { get; set; } = default!;
    [Inject] private IStringLocalizer<LeadingCompanies> Localizer { get; set; } = default!;
    private IJSObjectReference? JsObjectReference { get; set; } = default;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            JsObjectReference =
                await JsRunTime.InvokeAsync<IJSObjectReference>
                    ("import", $"/Components/Home/{nameof(LeadingCompanies)}.razor.js");
        else
            await JsObjectReference!.InvokeVoidAsync("loadSlider");
    }
}
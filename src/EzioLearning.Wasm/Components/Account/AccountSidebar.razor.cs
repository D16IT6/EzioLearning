using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountSidebar : AccountComponentBase
    {
        [Inject] private IStringLocalizer<AccountSidebar> Localizer { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
    }
}

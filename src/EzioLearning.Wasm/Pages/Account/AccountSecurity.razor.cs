using EzioLearning.Wasm.Components.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;


namespace EzioLearning.Wasm.Pages.Account
{
    public partial class AccountSecurity : AccountComponentBase
    {
        [Inject] private IStringLocalizer<AccountSecurity> Localizer { get; set; } = default!;
    }
}

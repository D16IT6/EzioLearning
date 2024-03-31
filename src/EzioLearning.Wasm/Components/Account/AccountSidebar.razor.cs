using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using EzioLearning.Share.Dto.Account;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountSidebar : AccountComponentBase
    {
        [Inject] private IStringLocalizer<AccountSidebar> Localizer { get; set; } = default!;
    }
}

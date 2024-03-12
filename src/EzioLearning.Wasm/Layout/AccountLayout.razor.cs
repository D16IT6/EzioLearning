using EzioLearning.Share.Dto.Account;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Layout
{
    public partial class AccountLayout
    {
        [CascadingParameter] public AccountInfoMinimalDto? AccountInfoMinimal { get; set; }

    }
}

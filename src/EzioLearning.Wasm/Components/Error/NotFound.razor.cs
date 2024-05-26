using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Error;

public partial class NotFound
{
    [Inject] private IStringLocalizer<NotFound> Localizer { get; set; } = default!;
}
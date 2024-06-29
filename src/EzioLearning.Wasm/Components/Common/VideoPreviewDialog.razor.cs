using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Common
{
    public partial class VideoPreviewDialog
    {
        [Parameter] public CourseLectureViewDto Lecture { get; set; } = new();

        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

    }
}

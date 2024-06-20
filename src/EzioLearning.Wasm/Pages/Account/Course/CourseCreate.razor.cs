using System.Security.Claims;
using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Utils;
using EzioLearning.Share.Validators.Common;
using EzioLearning.Wasm.Dto.Learning.Course;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using MudExtensions;
using SlugGenerator;
using Syncfusion.Blazor.RichTextEditor;

namespace EzioLearning.Wasm.Pages.Account.Course
{
    public partial class CourseCreate : AccountComponentBase, IAsyncDisposable
    {
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        private IJSObjectReference JsObjectReference { get; set; } = default!;

        [Inject] private IStringLocalizer<CourseCreate> Localizer { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;

        [Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
        [Inject] private ICourseService CourseService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private ICollection<CourseCategoryViewDto> CourseCategories { get; set; } = new List<CourseCategoryViewDto>();
        private IEnumerable<CourseCategoryViewDto> SelectedCourseCategories { get; set; } = [];

        private RenderFragment? CourseCategoriesRenderFragment { get; set; }

        [SupplyParameterFromForm] private CourseCreateDto CourseCreateDto { get; set; } = new();

        private static readonly List<ToolbarItemModel> Tools =
        [
            new ToolbarItemModel() { Command = ToolbarCommand.FontName },
            new ToolbarItemModel() { Command = ToolbarCommand.FontSize },
            new ToolbarItemModel() { Command = ToolbarCommand.FontColor },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.Bold },
            new ToolbarItemModel() { Command = ToolbarCommand.Italic },
            new ToolbarItemModel() { Command = ToolbarCommand.Underline },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.Formats },
            new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
            new ToolbarItemModel() { Command = ToolbarCommand.OrderedList },
            new ToolbarItemModel() { Command = ToolbarCommand.UnorderedList },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.CreateLink },
            new ToolbarItemModel() { Command = ToolbarCommand.Image },
            new ToolbarItemModel() { Command = ToolbarCommand.Video },
            new ToolbarItemModel() { Command = ToolbarCommand.Audio },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.CreateTable },
            new ToolbarItemModel() { Command = ToolbarCommand.SourceCode },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.Undo },
            new ToolbarItemModel() { Command = ToolbarCommand.Redo },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        ];

        public string? ImagePreviewUrl { get; set; }


        private CourseSectionCreateBlazorDto CourseSectionCreateDto { get; set; } = new();

        private int ActiveTab { get; set; }
        private bool BaseInfoSuccess { get; set; }

        private List<CourseSectionCreateBlazorDto> CourseSections { get; } = new();


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CourseCategories = (await CourseCategoryService.GetCourseCategories());

            CourseCategoriesRenderFragment = RecursiveSelect(CourseCategories.ToList());

            await RestoreModel();


            JsObjectReference =
                await JsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    $"/{nameof(Pages)}/{nameof(Account)}/{nameof(Course)}/{nameof(CourseCreate)}.razor.js");
        }

        private async Task RestoreModel()
        {
            var courseCreateDto = (await LocalStorageService.GetItemAsync<CourseCreateDto>(nameof(CourseCreateDto)));
            CourseCreateDto = courseCreateDto ?? new();

            SelectedCourseCategories = CourseCategories
                .Where(x => CourseCreateDto.CourseCategoryIds.Contains(x.Id))
                .ToArray();
        }

        private RenderFragment RecursiveSelect(List<CourseCategoryViewDto> categories, Guid? parentId = null)
        {
            return builder =>
            {
                var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

                if (!childCategories.Any()) return;

                foreach (var childCategory in childCategories)
                {
                    var paddingText = $"{childCategory.Name}";
                    var subChildCategories = categories
                        .Where(x => x.ParentId == childCategory.Id).ToList();

                    if (subChildCategories.Any())
                    {
                        builder.OpenComponent<MudSelectItemGroupExtended<CourseCategoryViewDto>>(0);
                        builder.AddAttribute(1, "T", typeof(CourseCategoryViewDto));
                        builder.AddAttribute(2, "Text", paddingText);
                        builder.AddAttribute(3, "Nested", true);
                        builder.AddAttribute(4, "Sticky", true);
                        builder.AddAttribute(5, "InitiallyExpanded", false);
                        builder.AddAttribute(6, "ChildContent", RecursiveSelect(categories, childCategory.Id));
                        builder.CloseComponent();
                    }
                    else
                    {
                        builder.OpenComponent<MudSelectItemExtended<CourseCategoryViewDto>>(0);
                        builder.AddAttribute(1, "T", typeof(CourseCategoryViewDto));
                        builder.AddAttribute(2, "Text", paddingText);
                        builder.AddAttribute(3, "Value", childCategory);
                        builder.CloseComponent();
                    }
                }

            };
        }

        private void ResetCourseCategories()
        {
            SelectedCourseCategories = [];
            CourseCreateDto.CourseCategoryIds = [];
            StateHasChanged();
        }

        private async Task CreateNewCourseSubmit()
        {
            CourseCreateDto.CreatedBy = Guid.Parse(AuthenticationState.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value);
            var response = await CourseService.CreateNewCourse(CourseCreateDto);
            if (response.IsSuccess)
            {
                await LocalStorageService.RemoveItemAsync(nameof(CourseCreateDto));
                if (response.Data != null) CourseCreateDto = response.Data;

                Snackbar.Add(response.Message, Severity.Success);
                BaseInfoSuccess = true;
                ActiveTab = 1;
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(response);
            }
        }
        private async Task OnCoursePosterChanged(CourseCreateDto _, InputFileChangeEventArgs obj)
        {
            var file = obj.File;

            ImagePreviewUrl = await file.GetBlobStream(JsRuntime);
        }
        private void CreateNewCourseSection()
        {
            var courseNames = CourseSections.Select(x => x.Name.GenerateSlug());
            if (courseNames.Contains(CourseSectionCreateDto.Name.GenerateSlug()))
            {
                Snackbar.Add("Tên phần không được giống nhau", Severity.Error);
                return;
            }
            CourseSections.Add(new CourseSectionCreateBlazorDto()
            {
                Name = CourseSectionCreateDto.Name,
                CourseId = CourseCreateDto.Id
            });
            CourseSectionCreateDto = new CourseSectionCreateBlazorDto()
            {
                CourseId = CourseCreateDto.Id
            };

            StateHasChanged();

        }
        private void CourseLessonUpdate(MudItemDropInfo<CourseLectureCreateBlazorDto> dropItem)
        {
            var draggedItem = dropItem.Item;

            if (draggedItem == null) return;

            var courseSection = CourseSections.FirstOrDefault(x => x.Id == draggedItem.CourseSectionId);
            if (courseSection == null) return;

            var dropContainer = courseSection.CourseLectureDropContainer;

            if (!dropContainer.HasTransactionIndexChanged()) return;

            var targetOrder = dropItem.IndexInZone;
            var draggedItemOriginalOrder = draggedItem.SortOrder;

            var lesson = courseSection.CourseLectures.First(lesson => lesson.SortOrder == targetOrder);

            lesson.SortOrder = draggedItemOriginalOrder;

            draggedItem.SortOrder = targetOrder;

            courseSection.CourseLectures = courseSection.CourseLectures.OrderBy(x => x.SortOrder).ToList();

            Console.WriteLine(string.Join(',', courseSection.CourseLectures.Select(x => $"{x.Name}")));


#pragma warning disable BL0005
            dropContainer.Items = courseSection.CourseLectures;
#pragma warning restore BL0005
            dropContainer.Refresh();
        }

        private void CreateNewLesson(CourseSectionCreateBlazorDto sectionCreate)
        {
            var courseLessonDto = new CourseLectureCreateBlazorDto
            {
                Name = $"Test Lesson {Random.Shared.Next(1000)}",
                SortOrder = sectionCreate.CourseLectures.Count(),
                LectureType = CourseLectureType.Video,
                CourseSectionId = sectionCreate.Id,

            };
            sectionCreate.CourseLectures.Add(courseLessonDto);
            UpdateLectureType(courseLessonDto, courseLessonDto.LectureType);
            StateHasChanged();

        }
        private bool ItemsSelector(CourseLectureCreateBlazorDto arg1, string arg2)
        {
            return arg1.CourseSectionId.ToString() == arg2;
        }

        private Task CourseDelete(CourseSectionCreateBlazorDto courseSectionCreateDto, CourseLectureCreateBlazorDto context)
        {
            courseSectionCreateDto.CourseLectures.Remove(context);

            StateHasChanged();
            return Task.CompletedTask;
        }
        private async Task OnCourseLectureFileChanged(CourseLectureCreateBlazorDto lectureCreate, InputFileChangeEventArgs inputFile)
        {
            try
            {
                var currentAcceptType = lectureCreate.FileUploadContainer.Accept;

                var acceptTypes = new List<string>();
                if (!string.IsNullOrWhiteSpace(currentAcceptType))
                {
                    acceptTypes = currentAcceptType.Split(',').ToList();
                }
                else
                {
                    acceptTypes.AddRange(lectureCreate.LectureType == CourseLectureType.Video
                        ? FileConstants.VideoAcceptTypes
                        : FileConstants.DocumentAcceptTypes);
                }

                var file = inputFile.File;
                var maxFileSize = lectureCreate.LectureType == CourseLectureType.Video
                    ? FileConstants.VideoUploadLimit
                    : FileConstants.DocumentUploadLimit;

                var fileExtension = Path.GetExtension(file.Name);
                if (file.Size >= maxFileSize || !acceptTypes.Contains(fileExtension))
                {
                    Snackbar.Add(
                        $"Vui lòng chọn file đúng loại định dạng:<br>" +
                        $"Video: {string.Join(',', FileConstants.VideoAcceptTypes)}, " +
                        $"<= {FileConstants.VideoUploadLimit.ShowFormatFileSize()}<br>" +
                        $"Document: {string.Join(',', FileConstants.DocumentAcceptTypes)}, " +
                        $"<= {FileConstants.DocumentUploadLimit.ShowFormatFileSize()}", Severity.Error
                    );

                    lectureCreate.FileUpload = null;
                    lectureCreate.TempFileUrl = "";
                    return;
                }

                lectureCreate.TempFileUrl = await file.GetBlobStream(JsRuntime);
            }
            catch (Exception)
            {
                Snackbar.Add("file quá nặng, không thể mở preview", Severity.Error);
            }

        }

        private Task UpdateLectureType(CourseLectureCreateBlazorDto courseLectureCreate, CourseLectureType target)
        {
            courseLectureCreate.LectureType = target;
            courseLectureCreate.FileUpload = null;
            courseLectureCreate.TempFileUrl = string.Empty;
            return Task.CompletedTask;
        }

        private Task DeleteLesson(CourseSectionCreateBlazorDto courseSectionDto)
        {
            CourseSections.Remove(courseSectionDto);
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void UpdateCourseCategories()
        {
            CourseCreateDto.CourseCategoryIds = SelectedCourseCategories.Select(x => x.Id).ToArray();
        }

        private async Task CreateCourseDetail()
        {

            foreach (var file in CourseSections.SelectMany(x=>x.CourseLectures).Select(x=>x.FileUpload))
            {
                if (file == null || file.Size >= FileConstants.VideoUploadLimit || file.Size <= 0)
                {
                    Snackbar.Add("File đầu vào không hợp lệ, vui lòng kiểm tra!");
                    return;
                }
            }

            var needLecture = !CourseSections.Any();

            foreach (var courseSection in CourseSections)
            {
                if (!courseSection.CourseLectures.Any())
                {
                    courseSection.Severity = Severity.Error;
                    needLecture = true;
                    StateHasChanged();
                }
            }

            if (needLecture)
            {
                Snackbar.Add("Phần học phải có bài học, nếu không hãy xoá đi!",Severity.Error);
                return;
            }

            var taskList = new List<Task<ResponseBaseWithData<CourseSectionCreateBlazorDto>>>();
            foreach (var courseSection in CourseSections)
            { 
                taskList.Add(CourseService.CreateCourseSection(courseSection));
            }

            bool redirect = true;
            while (taskList.Any())
            {
                var finishedTask = await Task.WhenAny(taskList);

                var result = await finishedTask;
                if (result.IsSuccess)
                {
                    var data = result.Data;
                    if (data != null)
                    {
                        var createdSection = CourseSections.FirstOrDefault(x => x.Name != null && x.Name.Equals(data.Name));
                        if (createdSection != null)
                        {
                            createdSection.Id = data.Id;
                            await JsObjectReference.InvokeVoidAsync("scrollToElementById", "TimeLineRef");
                            StateHasChanged();
                        }
                    }
                }
                else
                {
                    SnackBarService.ShowErrorFromResponse(result);
                    redirect = false;

                }
                taskList.Remove(finishedTask);

            }

            if (redirect)
            {
                await NavigationService.Navigate(RouteConstants.Index, "Tạo khoá học hoàn tất!", 1, false,
                    Severity.Success);
            }
        }

        public async ValueTask DisposeAsync()
        {
            CourseCreateDto.CourseCategoryIds = SelectedCourseCategories.Select(x => x.Id).ToArray();

            await LocalStorageService.SetItemAsync(nameof(CourseCreateDto), CourseCreateDto);
        }


    }


}
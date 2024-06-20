using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace EzioLearning.Wasm.Dto.Learning.Course
{
    public class CourseSectionCreateBlazorDto : CourseSectionCreateDto
    {
        public List<CourseLectureCreateBlazorDto> CourseLectures { get; set; } = [];

        public Severity Severity { get; set; }

        public MudDropContainer<CourseLectureCreateBlazorDto> CourseLectureDropContainer { get; set; } = new();
    }

    public class CourseLectureCreateBlazorDto : CourseLectureCreateDto
    {
        public IBrowserFile? FileUpload { get; set; }

        public MudFileUpload<IBrowserFile> FileUploadContainer { get; set; } = new();
    }

}

using EzioLearning.Share.Utils;
using EzioLearning.Share.Validators.Common;
using EzioLearning.Share.Validators.Learning.Course;
using EzioLearning.Wasm.Dto.Learning.Course;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Validators.Learning.Course
{
    public class CourseSectionCreateBlazorDtoValidator : AbstractValidator<CourseSectionCreateBlazorDto>
    {
        public CourseSectionCreateBlazorDtoValidator(IStringLocalizer<CourseSectionCreateBlazorDtoValidator> localizer,
            CourseSectionCreateDtoValidator validator)
        {
            Include(validator);
        }
    }

    public class CourseLectureCreateBlazorDtoValidator : AbstractValidator<CourseLectureCreateBlazorDto>
    {
        public CourseLectureCreateBlazorDtoValidator(IStringLocalizer<CourseLectureCreateBlazorDtoValidator> localizer,
            CourseLectureCreateDtoValidator validator)
        {
            Include(validator);

            RuleFor(x => x.FileUpload)
                .NotNull().WithMessage("Vui lòng chọn file")
                .Must(file => file is { Size: > 0 }).WithMessage("File bị lỗi, vui lòng chọn lại");

            RuleFor(x => x.FileUpload)
                .Cascade(CascadeMode.Stop)
                .Must(fileUpload =>
                {
                    if (fileUpload == null) return false;
                    var fileExtension = Path.GetExtension(fileUpload.Name);
                    return
                        FileConstants.VideoAcceptTypes.Contains(fileExtension) &&
                        fileUpload.Size <= FileConstants.VideoUploadLimit;
                })
                .When(x => x.LectureType == CourseLectureType.Video)
                .WithMessage("Dung lượng quá lớn hoặc sai định dạng.");

            RuleFor(x => x.FileUpload)
                .Cascade(CascadeMode.Stop)
                .Must(fileUpload =>
                {
                    if (fileUpload == null) return false;
                    var fileExtension = Path.GetExtension(fileUpload.Name);
                    return
                        FileConstants.DocumentAcceptTypes.Contains(fileExtension) &&
                        fileUpload.Size <= FileConstants.DocumentUploadLimit;
                })
                .When(x => x.LectureType == CourseLectureType.Document)
                .WithMessage("Dung lượng quá lớn hoặc sai định dạng.");
        }
    }
}

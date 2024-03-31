using AutoMapper;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Dto.Learning.CourseCategory;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;

namespace EzioLearning.Core.Dto;

public class MapperClass : Profile
{

    public MapperClass()
    {
        CreateMap<CourseCreateApiDto, Course>();
        CreateMap<CourseCategoryViewDto, CourseCategory>();

        CreateMap<CourseCategory, CourseCategoryViewDto>()
            .ForMember(dest => dest.ParentName,
                otp =>
                    otp.MapFrom(src => src.Parent!.Name));

        CreateMap<TopCourseCategoryDto, CourseCategory>().ReverseMap();
        CreateMap<CourseCategoryCreateApiDto, CourseCategory>().ForMember(x => x.Image, opt =>
            opt.Ignore());

        CreateMap<CourseCategory, CourseCategoryCreateApiDto>().ForMember(x => x.Image, opt =>
            opt.Ignore());

        CreateMap<CourseCreateDto, Course>();

        CreateMap<AppUser, InstructorViewDto>()
            .ForMember(x => x.Name, cfg => cfg.MapFrom(x => $"{x.FirstName} {x.LastName}"))
            .ForMember(x => x.StudentCount, cfg => cfg.MapFrom(x => x.Students.Count));

        CreateMap<UserCreateApiDto, AppUser>()
            .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        CreateMap<AppUser, UserCreateApiDto>()
            .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        CreateMap<RegisterRequestApiDto, AppUser>()
            .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        CreateMap<AppUser, RegisterRequestApiDto>()
            .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        CreateMap<AppUser, AccountInfoMinimalDto>();
        CreateMap<AppUser, AccountInfoDto>().ForMember(x => x.FullName, opt => opt.Ignore());
    }

}

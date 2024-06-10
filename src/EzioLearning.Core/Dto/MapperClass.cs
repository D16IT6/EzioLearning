using System.Globalization;
using AutoMapper;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Dto.Learning.CourseCategory;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Domain.Entities.System;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Dto.Culture;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;

namespace EzioLearning.Core.Dto;

public class MapperClass : Profile
{

    public MapperClass()
    {

        CreateMap<Culture, CultureViewDto>().ForMember(x => x.ImageUrl, cfg => cfg.Ignore());


        CreateMap<CourseCategoryViewDto, CourseCategory>();

        CreateMap<CourseCategory, CourseCategoryViewDto>()
            .ForMember(dest => dest.ParentName,
                otp =>
                    otp.MapFrom(src => src.Parent!.CourseCategoryTranslations.First(x => CultureInfo.CurrentCulture.Name == x.CultureId).Name))
            .ForMember(dest => dest.Name,
                otp =>
                    otp.MapFrom(src => src.CourseCategoryTranslations.First(x => CultureInfo.CurrentCulture.Name == x.CultureId).Name));

        CreateMap<TopCourseCategoryDto, CourseCategory>().ReverseMap()
            .ForMember(dest => dest.Name,
                otp =>
                    otp.MapFrom(src => src.CourseCategoryTranslations.First(x => CultureInfo.CurrentCulture.Name == x.CultureId).Name));

        CreateMap<CourseCategoryCreateApiDto, CourseCategory>().ForMember(x => x.Image, opt =>
            opt.Ignore());

        CreateMap<CourseCategory, CourseCategoryCreateApiDto>().ForMember(x => x.Image, opt =>
            opt.Ignore())
            .ForMember(dest => dest.Name,
                otp =>
                    otp.MapFrom(src => src.CourseCategoryTranslations.First(x => CultureInfo.CurrentCulture.Name == x.CultureId).Name));

        CreateMap<CourseCreateApiDto, Course>();

        CreateProjection<Course, CourseViewDto>()
            .ForMember(x => x.TeacherAvatar,
            otp
                => otp.MapFrom(u => u.User!.Avatar))
            .ForMember(x => x.TeacherName,
                otp
                    => otp.MapFrom(u => u.User!.FullName));

        CreateProjection<AppUser, UserDto>();

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
            .ForMember(x => x.BrowserFile, cfg => cfg.Ignore());
        CreateMap<AppUser, AccountInfoMinimalDto>();
        CreateMap<AppUser, AccountInfoDto>().ForMember(x => x.FullName, opt => opt.Ignore());
    }

}

using AutoMapper;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Auth;

public class RegisterRequestApiDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }

    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? ConfirmPassword { get; init; }
    public string? PhoneNumber { get; init; }

    public DateTime? DateOfBirth { get; init; } = DateTime.UtcNow.AddYears(-10);

    public IFormFile? Avatar { get; init; }

    public string? LoginProvider { get; init; }
    public string? ProviderName { get; init; }
    public string? ProviderKey { get; init; }


    public class RegisterRequestDtoProfile : Profile
    {
        public RegisterRequestDtoProfile()
        {
            CreateMap<RegisterRequestApiDto, AppUser>()
                .ForMember(x => x.Avatar, cfg => cfg.Ignore());
            CreateMap<AppUser, RegisterRequestApiDto>()
                .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        }
    }
}
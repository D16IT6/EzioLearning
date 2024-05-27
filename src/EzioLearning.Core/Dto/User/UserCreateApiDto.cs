using EzioLearning.Share.Dto.User;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.User
{
    public class UserCreateApiDto : UserCreateDto
    {
        public new IFormFile? Avatar { get; init; }

    }
}

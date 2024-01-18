using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.DTO;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Core.Repositories
{
    public interface IUserRepository : IPagedRepository<AppUser, Guid, UserDto>
    {

    }
}

using AutoMapper;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EzioLearning.Api.Hubs
{
    public class TestHub(UserManager<AppUser> userManager,IMapper mapper) : Hub<ITestHubClient>
    {
        private Task<IEnumerable<UserDto>> GetUsers()
        {
            return Task.FromResult<IEnumerable<UserDto>>(mapper.ProjectTo<UserDto>(userManager.Users));
        }

        public async Task SendUsers()
        {
            var users = await GetUsers();
            await Clients.All.ReceiveUsers(users.ToList());
        }
    }
}

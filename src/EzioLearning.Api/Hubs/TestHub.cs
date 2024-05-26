using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EzioLearning.Api.Hubs
{
    public class TestHub : Hub
    {
        public async Task SendUserCount(UserManager<AppUser> userManager)
        {
            var count = userManager.Users.Count(x => !x.IsDeleted);

            await Clients.Caller.SendAsync("ReceiveUserCount", count);
        }
    }
}

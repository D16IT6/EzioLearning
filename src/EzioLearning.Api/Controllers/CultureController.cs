using EzioLearning.Share.Dto.Culture;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CultureController(IStringLocalizer<CultureController> localizer) : ControllerBase
    {
        private const string BasePath = "/Uploads/Images/Cultures/";

        [HttpGet]
        public Task<IActionResult> GetCultures()
        {
            var data = new List<CultureViewDto>()
            {
                new() { Id=1,Culture = "vi-VN", ImageUrl = $"{BasePath}VietNam.png" },
                new() { Id=2,Culture = "en-US", ImageUrl = $"{BasePath}US.png" }
            };
            return Task.FromResult<IActionResult>(Ok(new ResponseBaseWithList<CultureViewDto>()
            {
                Message = localizer.GetString("GetCultureSuccess"),
                Data = data
            }));
        }
    }
}

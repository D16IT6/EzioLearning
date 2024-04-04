using EzioLearning.Core.Repositories.System;
using EzioLearning.Share.Dto.Culture;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CultureController(IStringLocalizer<CultureController> localizer,ICultureRepository cultureRepository) : ControllerBase
    {
        private const string BasePath = "/Uploads/Images/Cultures/";

        [HttpGet]
        public async Task<IActionResult> GetCultures()
        {
            var cultures = await cultureRepository.GetAllAsync();

            return Ok(new ResponseBaseWithList<CultureViewDto>()
            {
                Message = localizer.GetString("GetCultureSuccess"),
                Data = cultures.Select(x=> 
                    new CultureViewDto()
                    {
                        Id = x.Id,
                        ImageUrl = BasePath + x.Id + ".png"
                    }).
                    ToList()
            });
        }
    }
}

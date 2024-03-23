using System.Net;
using System.Security.Claims;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Api.Filters;

public class VerifyToken : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var memoryCache = context.HttpContext.RequestServices.GetService<CacheService>();

        var sessionId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value ??
                        string.Empty;

        var localizer = context.HttpContext.RequestServices.GetService<IStringLocalizer<VerifyToken>>();

        var token = memoryCache?.Get<string>(sessionId, CacheConstant.AccessToken);

        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(token))
            context.Result = new BadRequestObjectResult(new ResponseBase
            {
                Message = localizer.GetString("TokenFake"),
                Status = HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string[]>
                {
                    { "Token", [localizer.GetString("TokenFake")] }
                }
            });
        base.OnActionExecuting(context);
    }
}
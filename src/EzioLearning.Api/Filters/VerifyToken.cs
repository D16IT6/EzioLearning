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

        var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<VerifyToken>>();

        var token = memoryCache?.Get<string>(sessionId, CacheConstant.AccessToken);

        var tokenFakeMessage = localizer.GetString("TokenFake");
        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(token))
            context.Result = new BadRequestObjectResult(new ResponseBase
            {
                Message = tokenFakeMessage,
                Status = HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string[]>
                {
                    { "Token", [tokenFakeMessage] }
                }
            });
        base.OnActionExecuting(context);
    }
}
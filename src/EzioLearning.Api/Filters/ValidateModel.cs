using System.Net;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EzioLearning.Api.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid) context.Result = new BadRequestObjectResult(
            new ResponseBase()
            {
                Status = HttpStatusCode.BadRequest,
                Errors = context.ModelState.ToDictionary(x => x.Key,
                    x => x.Value?.Errors
                        .Select(x => x.ErrorMessage).ToArray())
            });
        base.OnActionExecuting(context);
    }
}
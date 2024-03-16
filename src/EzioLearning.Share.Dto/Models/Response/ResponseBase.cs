using System.Net;

namespace EzioLearning.Share.Models.Response;

//Merge with fluent validator
public class ResponseBase
{
    public string Type { get; set; } = HttpResponseType.Ok;
    public string Title { get; set; } = "Thành công";
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
    public string? Message { get; set; }

    public bool IsSuccess
    {
        get
        {
            Type = Status switch
            {
                HttpStatusCode.BadRequest => HttpResponseType.BadRequest,
                HttpStatusCode.Forbidden => HttpResponseType.Forbidden,
                HttpStatusCode.Unauthorized => HttpResponseType.Unauthorized,
                HttpStatusCode.InternalServerError => HttpResponseType.InternalServerError,
                _ => Type
            };

            var statusCode = (int)Status;
            
            return statusCode < 400;
        }
    }

    public Dictionary<string, string[]> Errors { get; init; } = [];

    public string TraceId { get; init; } = string.Empty;
}
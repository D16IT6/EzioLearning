using System.Net;

namespace EzioLearning.Share.Models.Response;

//Merge with fluent validator
public class ResponseBase
{
    public string Type { get; set; } = HttpResponseType.Ok;
    public string Title { get; init; } = string.Empty;
    public HttpStatusCode Status { get; init; } = HttpStatusCode.BadRequest;
    public string? Message { get; init; }

    public bool IsSuccess
    {
        get
        {
            switch (Status)
            {
                case HttpStatusCode.BadRequest:
                    Type = HttpResponseType.BadRequest;
                    break;
                case HttpStatusCode.Forbidden:
                    Type = HttpResponseType.Forbidden;
                    break;
                case HttpStatusCode.Unauthorized:
                    Type = HttpResponseType.Unauthorized;
                    break;
            }

            var statusCode = (int)Status;
            
            return statusCode < 400;
        }
    }

    public Dictionary<string, string[]> Errors { get; init; } = [];

    public string TraceId { get; init; } = string.Empty;
}
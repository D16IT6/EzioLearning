using System.Net;

namespace EzioLearning.Core.Models.Response
{
    public class ResponseBase
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }

        public bool IsSuccess
        {
            get
            {
                int statusCode = (int)StatusCode;
                return statusCode < 400;
                ;
            }
        }
    }
}

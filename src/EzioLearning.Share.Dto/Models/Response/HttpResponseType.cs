namespace EzioLearning.Share.Models.Response
{
    public record HttpResponseType
    {
        public const string Ok = "https://datatracker.ietf.org/doc/html/rfc2616#section-10.2.1";
        public const string Created = "https://datatracker.ietf.org/doc/html/rfc2616#section-10.2.2";

        public const string BadRequest = "https://datatracker.ietf.org/doc/html/rfc2616#section-10.4.1";
        public const string Unauthorized = "https://datatracker.ietf.org/doc/html/rfc2616#section-10.4.2";
        public const string Forbidden = "https://datatracker.ietf.org/doc/html/rfc2616#section-10.4.3";
        public const string InternalServerError = "https://datatracker.ietf.org/doc/html/rfc2616#section-10.5.1";

    }
}

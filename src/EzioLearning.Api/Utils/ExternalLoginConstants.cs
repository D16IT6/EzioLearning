namespace EzioLearning.Api.Utils
{
    public record ExternalLoginConstants
    {
        public static string Google = nameof(Google);
        public static string Facebook = nameof(Facebook);
        public static string Microsoft = nameof(Microsoft);
        public static string CallBackPath = "/api/Auth/CallBack";
    }
}

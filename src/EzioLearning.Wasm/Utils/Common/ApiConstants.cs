namespace EzioLearning.Wasm.Utils.Common;

public abstract record ApiConstants
{
    public static string BaseUrl { get; set; } = "https://localhost:37000/";
    public static string ApiAuthenticationType { get; set; } = "ApiAuth";
}
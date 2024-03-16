namespace EzioLearning.Wasm.Utils.Common;

public abstract record ApiConstants
{
    public static string BaseUrl { get; set; } = "https://localhost:7000/";
    public static string ApiAuthenticationType { get; set; } = "ApiAuth";
}
namespace EzioLearning.Wasm.Providers
{
	public record ApiConstants
	{
		public static string BaseUrl { get; set; } = "https://localhost:7000/";
		public static string ApiAuthenticationType { get; set; } = "ApiAuth";
	}
}

using System.Text.Json;

namespace EzioLearning.Blazor.Client.Providers
{
    public static class JsonCommonOptions
    {
        public static JsonSerializerOptions DefaultSerializer = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}

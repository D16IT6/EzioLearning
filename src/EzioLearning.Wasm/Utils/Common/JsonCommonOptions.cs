using System.Text.Json;

namespace EzioLearning.Wasm.Utils.Common;

public static class JsonCommonOptions
{
    public static JsonSerializerOptions DefaultSerializer = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
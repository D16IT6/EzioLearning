using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Utils.Common;
using System.Text.Json;

namespace EzioLearning.Wasm.Utils.Extensions
{
    public static class ResponseExtensions
    {
        public static async Task<T> GetResponse<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null) 
            where T : ResponseBase
        {
            options ??= JsonCommonOptions.DefaultSerializer;

            await using var stream = await response.Content.ReadAsStreamAsync();
            var responseData =
                await JsonSerializer.DeserializeAsync<T>(stream,
                    options);

            return responseData!;
        }
    }
}

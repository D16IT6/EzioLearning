using EzioLearning.Share.Dto.Culture;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Services.Implement
{
    public class CultureService(HttpClient httpClient) : ICultureService
    {
        public async Task<ResponseBaseWithList<CultureViewDto>> GetCultures()
        {
            var response = await httpClient.GetAsync("/api/Culture");

            return await response.GetResponse<ResponseBaseWithList<CultureViewDto>>();

        }
    }
}

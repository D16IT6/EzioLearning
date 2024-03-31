using EzioLearning.Share.Dto.Culture;
using EzioLearning.Share.Models.Response;

namespace EzioLearning.Wasm.Services.Interface
{
    public interface ICultureService : IServiceBase
    {
        Task<ResponseBaseWithList<CultureViewDto>> GetCultures();
    }
}

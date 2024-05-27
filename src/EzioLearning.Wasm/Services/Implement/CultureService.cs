﻿using EzioLearning.Share.Dto.Culture;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Services.Implement
{
    public class CultureService(HttpClient httpClient) : ICultureService
    {
        private static readonly List<CultureViewDto> Cultures = new();
        public async Task<List<CultureViewDto>> GetCultures()
        {
            if (Cultures.Any()) return Cultures;
            var response = await httpClient.GetAsync("/api/Culture");

            var responseData = await response.GetResponse<ResponseBaseWithList<CultureViewDto>>();
            if (responseData.Data != null && responseData.Data.Any())
            {
                Cultures.AddRange(responseData.Data);
            }

            return Cultures;
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Smooth.Functions.Application.Configuration;
using Smooth.Functions.Application.Contracts;
using Smooth.Shared.Application.Configuration;
using Smooth.Shared.Application.Constants;

namespace Smooth.Functions.Infrastructure.Services;

public class ConfigService : IConfigService
{
    private readonly IApiService _apiService;
    private readonly IMemoryCache _memoryCache;
    private const string CACHE_KEY = "AllowedFileTypeOptions";

    public ConfigService(IApiServiceFactory apiServiceFactory, IMemoryCache memoryCache)
    {
        _apiService = apiServiceFactory.Create(ApiName.SMOOTH_SHOP);
        _memoryCache = memoryCache;
    }

    public async Task<List<AllowedFileTypeOptions>> GetAllowedFileTypeOptionsList()
    {
        if (!_memoryCache.TryGetValue(CACHE_KEY, out List<AllowedFileTypeOptions>? allowedFileTypeOptionsList))
        {
            var response = await _apiService.GetDataAsync<List<AllowedFileTypeOptions>>("config/allowedfiletypes");

            allowedFileTypeOptionsList = response ?? new List<AllowedFileTypeOptions>();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _memoryCache.Set(CACHE_KEY, allowedFileTypeOptionsList, cacheEntryOptions);
        }

        return allowedFileTypeOptionsList ?? [];
    }
}

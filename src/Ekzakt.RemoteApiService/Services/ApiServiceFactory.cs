using Microsoft.Extensions.Logging;

namespace Ekzakt.RemoteApiService.Services;

public class ApiServiceFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiService> _logger;

    public ApiServiceFactory(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public ApiService Create(string clientName)
    {
        var httpClient = _httpClientFactory.CreateClient(clientName);
        return new ApiService(httpClient, _logger);
    }
}
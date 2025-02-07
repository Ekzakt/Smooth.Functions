using Microsoft.Extensions.Logging;
using Smooth.Functions.Application.Contracts;

namespace Smooth.Functions.Infrastructure.Services;

public class ApiServiceFactory : IApiServiceFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiService> _logger;

    public ApiServiceFactory(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public IApiService Create(string clientName)
    {
        var httpClient = _httpClientFactory.CreateClient(clientName);
        return new ApiService(httpClient, _logger);
    }
}
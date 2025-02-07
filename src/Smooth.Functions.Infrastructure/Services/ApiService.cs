using Microsoft.Extensions.Logging;
using Smooth.Functions.Application.Contracts;
using System.Text;
using System.Text.Json;

namespace Smooth.Functions.Infrastructure.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task PostDataAsync<T>(string route, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Sending POST request to {route}");

            var response = await _httpClient.PostAsync(route, content);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending POST request: {ex.Message}");
        }
    }


    public async Task<T?> GetDataAsync<T>(string route, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            if (queryParams != null && queryParams.Any())
            {
                var queryString = string.Join("&", queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
                route = $"{route}?{queryString}";
            }

            _logger.LogInformation($"Sending GET request to {route}");


            var response = await _httpClient.GetAsync(route);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending GET request: {ex.Message}");
            throw;
        }
    }
}
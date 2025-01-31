using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Ekzakt.RemoteApiService.Services;

public class ApiService 
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<bool> PostDataAsync<T>(string url, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Sending POST request to {url}");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("POST request successful.");
                return true;
            }
            else
            {
                var x = response.ReasonPhrase;
                _logger.LogError($"POST request failed with status code: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending POST request: {ex.Message}");
            return false;
        }
    }
}
namespace Smooth.Functions.Application.Contracts;

public interface IApiService
{
    Task<T?> GetDataAsync<T>(string route, Dictionary<string, string>? queryParams = null);

    Task PostDataAsync<T>(string route, T data);
}
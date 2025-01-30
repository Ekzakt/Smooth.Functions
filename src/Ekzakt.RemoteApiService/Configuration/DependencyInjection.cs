using Microsoft.Extensions.DependencyInjection;
using Ekzakt.RemoteApiService.Services;

namespace Ekzakt.RemoteApiService.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddRemoteApiServices(this IServiceCollection services, Dictionary<string, string> clientConfigurations)
    {
        services.AddSingleton<ApiServiceFactory>();

        foreach (var (clientName, baseUrl) in clientConfigurations)
        {
            services.AddHttpClient<ApiService>(clientName, client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });
        }

        return services;
    }
}

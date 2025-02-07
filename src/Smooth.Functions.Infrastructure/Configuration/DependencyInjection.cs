using Microsoft.Extensions.DependencyInjection;
using Smooth.Functions.Application.Contracts;
using Smooth.Functions.Infrastructure.Services;

namespace Smooth.Functions.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddRemoteApiServices(this IServiceCollection services, Dictionary<string, string> clientConfigurations)
    {
        services.AddScoped<IFileValidator, FileValidator>();
        services.AddScoped<IFileService, AzureBlobService>();
        services.AddScoped<IConfigService, ConfigService>();
        services.AddScoped<IApiServiceFactory, ApiServiceFactory>();
        services.AddScoped<IApiService, ApiService>();

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

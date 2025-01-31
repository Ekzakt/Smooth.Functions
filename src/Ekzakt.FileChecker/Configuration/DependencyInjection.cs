using Ekzakt.FileChecker.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Ekzakt.FileChecker.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddFileChecker(this IServiceCollection services)
    {
        services.AddScoped<IFileChecker, Services.FileChecker>();

        return services;
    }
}

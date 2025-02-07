using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Smooth.Functions.Application.Configuration;
using Smooth.Functions.Application.Managers;
using Smooth.Functions.Infrastructure.Configuration;
using Smooth.Shared.Application.Constants;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

var containerClientConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
ArgumentNullException.ThrowIfNull(containerClientConnectionString, nameof(containerClientConnectionString));

builder.Services
    .AddOptions<FunctionOptions>()
    .BindConfiguration(FunctionOptions.SECTION_NAME);

builder.Services.AddScoped<IMediumManager, MediumManager>();

builder.Services
    .AddAzureClients(clientBuilder => {
        clientBuilder
            .AddBlobServiceClient(containerClientConnectionString);
    });

var apiUrl = builder.Configuration.GetValue<string>(ApiName.SMOOTH_SHOP);
builder.Services.AddRemoteApiServices(new Dictionary<string, string>
{
    { ApiName.SMOOTH_SHOP, apiUrl ?? string.Empty }
});

builder.Build().Run();

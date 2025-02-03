using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Ekzakt.RemoteApiService.Configuration;
using Ekzakt.FileChecker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Azure;
using Smooth.Functions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddFileChecker();

builder.Services
    .AddOptions<FunctionOptions>()
    .BindConfiguration(FunctionOptions.SECTION_NAME);

var containerClientConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
if (string.IsNullOrWhiteSpace(containerClientConnectionString))
{
    throw new InvalidOperationException("AzureWebJobsStorage is not configured properly.");
}

builder.Services
    .AddAzureClients(clientBuilder => {
        clientBuilder
            .AddBlobServiceClient(containerClientConnectionString);
    });

var apiUrl = builder.Configuration.GetValue<string>("ApiUrl");
builder.Services.AddRemoteApiServices(new Dictionary<string, string>
{
    { "SmoothShopApi", apiUrl ?? string.Empty }
});

builder.Build().Run();

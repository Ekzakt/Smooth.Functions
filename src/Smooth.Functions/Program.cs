using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Ekzakt.RemoteApiService.Configuration;
using Ekzakt.FileChecker.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddFileChecker();

builder.Services.AddRemoteApiServices(new Dictionary<string, string>
{
    { "SmoothShopApi", "https://shop.smooth.local" }
});

builder.Build().Run();

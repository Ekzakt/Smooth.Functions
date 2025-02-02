using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Ekzakt.RemoteApiService.Configuration;
using Ekzakt.FileChecker.Configuration;
using Microsoft.Extensions.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddFileChecker();

var apiUrl = builder.Configuration.GetValue<string>("ApiUrl");
builder.Services.AddRemoteApiServices(new Dictionary<string, string>
{
    { "SmoothShopApi", apiUrl ?? string.Empty }
});

builder.Build().Run();

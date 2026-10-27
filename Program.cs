using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

var blobServiceUri =
    new Uri("https://learningazurebed7.blob.core.windows.net");

builder.Services.AddSingleton(
    _ => new BlobServiceClient(
        blobServiceUri,
        new DefaultAzureCredential()));

builder.Build().Run();

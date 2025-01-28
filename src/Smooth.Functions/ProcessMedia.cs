using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Smooth.Functions
{
    public class ProcessMedia
    {
        private readonly ILogger<ProcessMedia> _logger;

        public ProcessMedia(ILogger<ProcessMedia> logger)
        {
            _logger = logger;

        }

        [Function(nameof(ProcessMedia))]
        public async Task Run(
            [BlobTrigger("%SourceContainerName%/{name}", 
            Connection = "AzureWebJobsStorage")] Stream stream,
            string name)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? string.Empty;
            var targetContainerName = Environment.GetEnvironmentVariable("TargetContainerName") ?? string.Empty;
            var sourceContainerName = Environment.GetEnvironmentVariable("SourceContainerName") ?? string.Empty;

            var blobServiceClient = new BlobServiceClient(connectionString);
            var sourceContainerClient = blobServiceClient.GetBlobContainerClient(sourceContainerName);
            var sourceBlobClient = sourceContainerClient.GetBlobClient(name);

            var targetContainerClient = blobServiceClient.GetBlobContainerClient(targetContainerName);
            await targetContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            var targetBlobClient = targetContainerClient.GetBlobClient(name);

            _logger.LogInformation($"Copying blob '{name}' from '{sourceContainerName}' to '{targetContainerName}'...");
            await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
            _logger.LogInformation($"Successfully copied blob '{name}' to container '{targetContainerName}'.");
        }
    }
}

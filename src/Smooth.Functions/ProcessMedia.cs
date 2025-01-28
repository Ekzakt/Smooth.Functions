using System.IO;
using System.Threading.Tasks;
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
            [BlobTrigger("new-media/{name}", 
            Connection = "AzureWebJobsStorage")] Stream stream,
            string name)
        {
            //using var blobStreamReader = new StreamReader(stream);
            //var content = await blobStreamReader.ReadToEndAsync();
            //_logger.LogInformation(
            //    $"C# Blob trigger function Processed blob.\n" +
            //    $"Name: {name}");


            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? string.Empty;

            // Source blob container and blob name
            string sourceContainerName = "new-media";
            string targetContainerName = "data";

            // Create BlobServiceClient
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get reference to the source container and blob
            var sourceContainerClient = blobServiceClient.GetBlobContainerClient(sourceContainerName);
            var sourceBlobClient = sourceContainerClient.GetBlobClient(name);

            // Get reference to the target container and blob
            var targetContainerClient = blobServiceClient.GetBlobContainerClient(targetContainerName);
            await targetContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            var targetBlobClient = targetContainerClient.GetBlobClient(name);

            // Copy blob content
            _logger.LogInformation($"Copying blob '{name}' from '{sourceContainerName}' to '{targetContainerName}'...");
            await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
            _logger.LogInformation($"Successfully copied blob '{name}' to container '{targetContainerName}'.");
        }
    }
}

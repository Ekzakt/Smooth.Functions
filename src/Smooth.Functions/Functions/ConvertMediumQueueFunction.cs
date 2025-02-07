using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using Smooth.Functions.Application.Configuration;

namespace Smooth.Functions.Functions
{
    public class ConvertMediumQueueFunction
    {
        private readonly ILogger<ConvertMediumQueueFunction> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public ConvertMediumQueueFunction(
            ILogger<ConvertMediumQueueFunction> logger,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }


        [Function(nameof(ConvertMediumQueueFunction))]
        public async Task Run(
            [BlobTrigger("%BlobContainerMediaProcessing%/{name}", 
            Connection = "")] Stream stream, 
            string name)
        {
            try
            {
                _logger.LogInformation($"Processing file: {name}");

                using Image image = Image.Load(stream);

                string webpFileName = Path.GetFileNameWithoutExtension(name) + ".webp";

                BlobContainerClient destinationContainer = _blobServiceClient.GetBlobContainerClient("webp");
                await destinationContainer.CreateIfNotExistsAsync(PublicAccessType.None);

                BlobClient outputBlob = destinationContainer.GetBlobClient(webpFileName);

                using var outputStream = new MemoryStream();
                var encoder = new WebpEncoder { Quality = 80 };
                image.Save(outputStream, encoder);
                outputStream.Position = 0;

                await outputBlob.UploadAsync(outputStream, overwrite: true);

                _logger.LogInformation($"Converted and saved {webpFileName} to 'data' container.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file {name}: {ex.Message}");
            }
        }
    }
}

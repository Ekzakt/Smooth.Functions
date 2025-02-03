using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Smooth.Functions.Functions
{
    public class ConvertToWebP
    {
        private readonly ILogger<ConvertToWebP> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public ConvertToWebP(
            ILogger<ConvertToWebP> logger,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }


        [Function(nameof(ConvertToWebP))]
        public async Task Run(
            [BlobTrigger("new-media/{name}", 
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

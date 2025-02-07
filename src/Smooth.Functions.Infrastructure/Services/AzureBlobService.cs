using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Smooth.Functions.Application.Contracts;

namespace Smooth.Functions.Infrastructure.Services;

public class AzureBlobService : IFileService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }


    public async Task CopyFileAsync(Stream fileStream, string targetContainerName, string targetFileName)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        fileStream.Position = 0;

        var targetBlobContainer = _blobServiceClient.GetBlobContainerClient(targetContainerName);
        var targetBlobClient = targetBlobContainer.GetBlobClient(targetFileName);

        await targetBlobContainer.CreateIfNotExistsAsync(PublicAccessType.None);
        await targetBlobClient.UploadAsync(fileStream, overwrite: true);
    }


    public async Task CopyFileAsync(string sourceContainerName, string targetContainerName, string sourceFileName, string? targetFileName = null)
    {
        var sourceContainerClient = _blobServiceClient.GetBlobContainerClient(sourceContainerName);
        var targetContainerClient = _blobServiceClient.GetBlobContainerClient(targetContainerName);
        var sourceBlobClient = sourceContainerClient.GetBlobClient(sourceFileName);
        var targetBlobClient = targetContainerClient.GetBlobClient(targetFileName ?? sourceFileName);

        if (!await sourceBlobClient.ExistsAsync())
        {
            throw new InvalidOperationException($"Blob '{sourceFileName}' not found in source container '{sourceContainerName}'.");
        }

        await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);

        while (true)
        {
            var properties = await targetBlobClient.GetPropertiesAsync();

            if (properties.Value.CopyStatus == CopyStatus.Success)
            {
                break;
            }

            await Task.Delay(100);
        }
        
        await sourceBlobClient.DeleteIfExistsAsync();
    }
}

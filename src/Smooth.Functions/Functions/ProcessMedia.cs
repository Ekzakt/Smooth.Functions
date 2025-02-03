using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ekzakt.FileChecker.Contracts;
using Ekzakt.RemoteApiService.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Smooth.Functions.Configuration;
using Smooth.Functions.Requests;

namespace Smooth.Functions.Functions;

public class ProcessMedia
{
    private readonly ILogger<ProcessMedia> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly FunctionOptions _functionOptions;
    private readonly ApiService _apiService;
    private readonly IFileChecker _fileChecker;

    public ProcessMedia(
        ILogger<ProcessMedia> logger,
        BlobServiceClient blobServiceClient,
        IOptions<FunctionOptions> functionOptions,
        ApiServiceFactory apiServiceFactory,
        IFileChecker fileChecker)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
        _functionOptions = functionOptions.Value;
        _apiService = apiServiceFactory.Create("SmoothShopApi");
        _fileChecker = fileChecker;
    }


    [Function(nameof(ProcessMedia))]
    public async Task Run(
        [BlobTrigger("%SourceContainerName%/{name}",
        Connection = "AzureWebJobsStorage")] Stream fileStream,
        string name)
    {
        long uploadFinishedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        bool isValid = await CheckFileAsync(fileStream, name);
        if (!isValid)
        {
            _logger.LogError($"File '{name}' is invalid.");
            return;
        }

        var apiUrl = Environment.GetEnvironmentVariable("ApiUrl") ?? string.Empty;
        var targetContainerName = Environment.GetEnvironmentVariable("TargetContainerName") ?? string.Empty;
        var sourceContainerName = Environment.GetEnvironmentVariable("SourceContainerName") ?? string.Empty;

        var sourceContainerClient = _blobServiceClient.GetBlobContainerClient(sourceContainerName);
        var sourceBlobClient = sourceContainerClient.GetBlobClient(name);

        var targetContainerClient = _blobServiceClient.GetBlobContainerClient(targetContainerName);
        await targetContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        var targetBlobClient = targetContainerClient.GetBlobClient(name);


        _logger.LogInformation($"Copying blob '{name}' from '{sourceContainerName}' to '{targetContainerName}'...");
        await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
        _logger.LogInformation($"Successfully copied blob '{name}' to container '{targetContainerName}'.");


        var result = await _apiService.PostDataAsync(
            url: "/file/confirm",
            data: new ConfirmUploadRequest
            {
                UploadedFileName = name,
                FileSize = sourceBlobClient.GetProperties().Value.ContentLength,
                UploadFinishedAt = uploadFinishedAt
            });

        _logger.LogInformation($"Upload confirmation result: {result}");
    }


    #region Helpers

    private async Task<bool> CheckFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            long fileSize = fileStream.Length;
            long maxSize = 10 * 1024 * 1024; // 10 MB
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".heif", ".mp4", ".mkv", ".avi", ".mov", ".webm", ".flv", ".wmv" };

            return await _fileChecker.CheckFileValidityAsync(fileStream, fileSize, fileName, maxSize, allowedExtensions);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking file '{fileName}': {ex.Message}");
            return false;
        }
    }

    #endregion Helpers
}

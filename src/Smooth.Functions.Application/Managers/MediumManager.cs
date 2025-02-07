using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Smooth.Functions.Application.Configuration;
using Smooth.Functions.Application.Contracts;
using Smooth.Functions.Application.Requests;
using Smooth.Shared.Application.Constants;

namespace Smooth.Functions.Application.Managers;

public class MediumManager : IMediumManager
{
    private readonly ILogger<MediumManager> _logger;
    private readonly IFileService _fileService;
    private readonly IOptions<FunctionOptions> _functionOptions;
    private readonly IFileValidator _fileValidator;
    private readonly IApiService _apiService;

    public MediumManager(
        ILogger<MediumManager> logger,
        IFileService fileService,
        IOptions<FunctionOptions> functionOptions,
        IApiServiceFactory apiServiceFactory,
        IFileValidator fileValidator)
    {
        _logger = logger;
        _fileService = fileService;
        _functionOptions = functionOptions;
        _fileValidator = fileValidator;
        _apiService = apiServiceFactory.Create(ApiName.SMOOTH_SHOP);
    }

    public async Task ValidateAndConvertMediumBlobTriggerAsync(Stream stream, string name)
    {
        try
        {
            _logger.LogInformation($"Executing function {nameof(ValidateAndConvertMediumBlobTriggerAsync)} file '{name}'");

            var isFileValid = await _fileValidator.ValidateFileStreamAsync(stream, name);

            var request = new ConfirmUploadRequest
            {
                UploadedFileName = name,
                FileSize = stream.Length,
                UploadFinishedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            await _apiService.PostDataAsync(
                route: "/file/confirm",
                data: request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error executing function {nameof(ValidateAndConvertMediumBlobTriggerAsync)} file '{name}': {ex.Message}");
            throw;
        }
    }
}

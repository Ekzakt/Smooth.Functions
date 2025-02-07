using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Smooth.Functions.Application.Managers;

namespace Smooth.Functions.Functions;

public class ValidateAndConvertMediumBlobFunction(
        ILogger<ValidateAndConvertMediumBlobFunction> _logger,
        IMediumManager _mediumManager
        )
{
    [Function(nameof(ValidateAndConvertMediumBlobFunction))]
    public async Task Run(
        [BlobTrigger("%BlobContainerMediaUploaded%/{name}",
        Connection = "AzureWebJobsStorage")] Stream stream,
        string name)
    {
        try
        {
            await _mediumManager.ValidateAndConvertMediumBlobTriggerAsync(stream, name);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

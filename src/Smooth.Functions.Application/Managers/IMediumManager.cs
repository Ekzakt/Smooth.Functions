
namespace Smooth.Functions.Application.Managers
{
    public interface IMediumManager
    {
        Task ValidateAndConvertMediumBlobTriggerAsync(Stream stream, string name);
    }
}
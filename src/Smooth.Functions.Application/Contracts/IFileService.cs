
namespace Smooth.Functions.Application.Contracts;

public interface IFileService
{
    Task CopyFileAsync(string sourceContainerName, string targetContainerName, string fileName, string? targetFileName = null);

    Task CopyFileAsync(Stream fileStream, string targetContainerName, string targetFileName);
}

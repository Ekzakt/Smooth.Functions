using Smooth.Functions.Application.Models;

namespace Smooth.Functions.Application.Contracts;

public interface IFileValidator
{
    Task<bool> ValidateFileStreamAsync(Stream fileStream, string fileName);
}

namespace Ekzakt.FileChecker.Contracts;

public interface IFileChecker
{
    Task<bool> CheckFileValidityAsync(Stream fileStream, long fileSize, string fileName, long maxSize, params string[] allowedExtensions);
}

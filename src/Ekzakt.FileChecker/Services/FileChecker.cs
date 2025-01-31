using Ekzakt.FileChecker.Contracts;
using Ekzakt.FileChecker.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ekzakt.FileChecker.Services;

public class FileChecker : IFileChecker
{
    private readonly ILogger<FileChecker> _logger;

    public FileChecker(ILogger<FileChecker> logger)
    {
        _logger = logger;
    }

    private static readonly Dictionary<string, byte[][]> _fileSignatures = new()
    {
        { ".jpg",  new byte[][] { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { ".jpeg", new byte[][] { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { ".png",  new byte[][] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { ".gif",  new byte[][] { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { ".bmp",  new byte[][] { new byte[] { 0x42, 0x4D } } },
        { ".tiff", new byte[][] { new byte[] { 0x49, 0x49, 0x2A, 0x00 }, new byte[] { 0x4D, 0x4D, 0x00, 0x2A } } },
        { ".webp", new byte[][] { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
        { ".heif", new byte[][] { new byte[] { 0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63 } } },
        { ".mp4",  new byte[][] { new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 } } },
        { ".mkv",  new byte[][] { new byte[] { 0x1A, 0x45, 0xDF, 0xA3 } } },
        { ".avi",  new byte[][] { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
        { ".mov",  new byte[][] { new byte[] { 0x00, 0x00, 0x00, 0x14, 0x66, 0x74, 0x79, 0x70 } } },
        { ".webm", new byte[][] { new byte[] { 0x1A, 0x45, 0xDF, 0xA3 } } },
        { ".flv",  new byte[][] { new byte[] { 0x46, 0x4C, 0x56 } } },
        { ".wmv",  new byte[][] { new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 } } }
    };

    public async Task<bool> CheckFileValidityAsync(Stream fileStream, long fileSize, string fileName, long maxSize, params string[] allowedExtensions)
    {
        _logger.LogInformation("Validating file: {FileName} with size {FileSize} bytes", fileName, fileSize);

        if (fileStream == null || fileSize == 0)
        {
            _logger.LogWarning("File stream is null or empty.");
            throw new ArgumentNullException(nameof(fileStream), "File stream cannot be null or empty.");
        }


        if (fileSize > maxSize)
        {
            _logger.LogWarning("File {FileName} exceeds the max allowed size ({MaxSize} bytes)", fileName, maxSize);
            throw new InvalidFileSizeException(fileSize, maxSize);
        }


        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !_fileSignatures.ContainsKey(extension))
        {
            _logger.LogWarning("File {FileName} has an unsupported or missing file extension.", fileName);
            throw new InvalidFileTypeException(fileName);
        }

        if (allowedExtensions.Length > 0 && !allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("File {FileName} has an extension '{Extension}' which is not in the allowed list.", fileName, extension);
            throw new InvalidFileTypeException(extension);
        }


        if (!await HasValidSignatureAsync(fileStream, extension))
        {
            _logger.LogWarning("File {FileName} failed the bitwise signature check.", fileName);
            throw new InvalidFileSignatureException(fileName);
        }

        _logger.LogInformation("File {FileName} passed all validation checks.", fileName);
        return true;
    }


    #region Helpers

    private async Task<bool> HasValidSignatureAsync(Stream fileStream, string extension)
    {
        if (!_fileSignatures.TryGetValue(extension, out byte[][]? validSignatures))
            return false;

        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] fileHeader = new byte[validSignatures.Max(s => s.Length)];
        int bytesRead = await fileStream.ReadAsync(fileHeader, 0, fileHeader.Length);

        return validSignatures.Any(signature =>
            bytesRead >= signature.Length && fileHeader.Take(signature.Length).SequenceEqual(signature));
    }


    #endregion Helpers
}

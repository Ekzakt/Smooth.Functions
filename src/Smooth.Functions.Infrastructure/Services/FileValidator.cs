using Microsoft.Extensions.Logging;
using Smooth.Functions.Application.Contracts;
using Smooth.Functions.Infrastructure.Exceptions;
using Smooth.Shared.Application.Configuration;

namespace Smooth.Functions.Infrastructure.Services;

public class FileValidator : IFileValidator
{
    private readonly ILogger<FileValidator> _logger;
    private readonly IConfigService _configService;
    public FileValidator(
        ILogger<FileValidator> logger,
        IConfigService configService)
    {
        _logger = logger;
        _configService = configService;
    }


    public static Dictionary<string, byte[][]> FileSignatures = new()
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


    public async Task<bool> ValidateFileStreamAsync(Stream fileStream, string fileName)
    {
        var fileTypeOptions = await _configService.GetAllowedFileTypeOptionsList();

        _logger.LogInformation("Validating file: {FileName}", fileName);

        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

        ValidateFileTypeOptions(fileTypeOptions);
        ValidateFunctionParameters(fileStream, fileName, fileTypeOptions);
        ValidateFileExtension(fileExtension, fileTypeOptions);
        ValidateFileSize(fileStream.Length, fileName, fileTypeOptions);

        await ValidateFileSignature(fileStream, fileExtension);

        _logger.LogInformation("File {FileName} passed all validation checks.", fileName);

        return true;
    }


    #region Helpers

    private void ValidateFileTypeOptions(List<AllowedFileTypeOptions> fileTypeOptions)
    {
        if (fileTypeOptions is null || fileTypeOptions.Count == 0)
        {
            throw new ArgumentNullException(nameof(fileTypeOptions), "Allowed file type options list is null or empty.");
        }
    }

    private void ValidateFileSize(long fileLength, string fileName, List<AllowedFileTypeOptions> fileTypes)
    {
        if (fileLength == 0)
        {
            throw new InvalidFileSizeException(fileName, fileLength, 0);
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedFileType = fileTypes.FirstOrDefault(ft => ft.AllowedExtensions.Contains(extension));

        if (fileLength > allowedFileType?.MaxFileSize)
        {
            throw new InvalidFileSizeException(fileName, fileLength, 0);
        }
    }


    private void ValidateFunctionParameters(Stream fileStream, string fileName, List<AllowedFileTypeOptions> fileTypes)
    {
        ArgumentNullException.ThrowIfNull(fileStream, nameof(fileStream));
        ArgumentNullException.ThrowIfNull(fileTypes, nameof(fileTypes));
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
    }


    private void ValidateFileExtension(string fileExtension, List<AllowedFileTypeOptions> allowedFileTypes)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileExtension, nameof(fileExtension));
        ArgumentNullException.ThrowIfNull(allowedFileTypes, nameof(allowedFileTypes));

        var isAllowed = allowedFileTypes.Any(x => x.AllowedExtensions.Contains(fileExtension));

        if (!isAllowed)
        {
            throw new InvalidFileExtensionException(fileExtension);
        }
    }


    private async Task ValidateFileSignature(Stream fileStream, string extension)
    {
        if (!FileSignatures.TryGetValue(extension, out byte[][]? validSignatures))
        {
            throw new FileExtensionNotInSignaturesListException(extension);
        }

        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] fileHeader = new byte[validSignatures.Max(s => s.Length)];
        int bytesRead = await fileStream.ReadAsync(fileHeader, 0, fileHeader.Length);

        var result = validSignatures.Any(signature =>
            bytesRead >= signature.Length && fileHeader.Take(signature.Length).SequenceEqual(signature));

        if (!result)
        {
            throw new InvalidFileSignatureException(extension);
        }
    }

    #endregion Helpers

}

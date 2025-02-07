namespace Smooth.Functions.Infrastructure.Exceptions;

[Serializable]
public class InvalidFileSizeException : Exception
{
    public InvalidFileSizeException()
        : base("The file size exceeds the allowed limit.") { }


    public InvalidFileSizeException(string fileName, long fileSize, long maxSize)
        : base($"The file {fileName} with size ({fileSize / (1024 * 1024)}MB) exceeds the maximum allowed limit of {maxSize / (1024 * 1024)}MB.") { }


    public InvalidFileSizeException(string fileName, long fileSize, long maxSize, Exception innerException)
        : base($"The file {fileName} with size ({fileSize / (1024 * 1024)}MB) exceeds the maximum allowed limit of {maxSize / (1024 * 1024)}MB.", innerException) { }
}
namespace Ekzakt.FileChecker.Exceptions;

public class InvalidFileSizeException : Exception
{
    public InvalidFileSizeException()
        : base("The file size exceeds the allowed limit.") { }


    public InvalidFileSizeException(long fileSize, long maxSize)
        : base($"The file size ({fileSize / (1024 * 1024)}MB) exceeds the maximum allowed limit of {maxSize / (1024 * 1024)}MB.") { }
}
namespace Smooth.Functions.Infrastructure.Exceptions;

[Serializable]
public class InvalidFileExtensionException : Exception
{
    public InvalidFileExtensionException()
        : base("The uploaded file extension is not supported.") { }


    public InvalidFileExtensionException(string fileType)
        : base($"The file extension '{fileType}' is not supported.") { }


    public InvalidFileExtensionException(string fileType, Exception innerException)
        : base($"The file extension '{fileType}' is not supported.", innerException) { }
}
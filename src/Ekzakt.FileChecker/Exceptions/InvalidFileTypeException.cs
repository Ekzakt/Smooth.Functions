namespace Ekzakt.FileChecker.Exceptions;

public class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException()
        : base("The uploaded file type is invalid.") { }


    public InvalidFileTypeException(string fileType)
        : base($"The file type '{fileType}' is not supported.") { }


    public InvalidFileTypeException(string fileType, Exception innerException)
        : base($"The file type '{fileType}' is not supported.", innerException) { }
}
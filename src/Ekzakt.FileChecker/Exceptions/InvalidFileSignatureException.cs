namespace Ekzakt.FileChecker.Exceptions;

public class InvalidFileSignatureException : Exception
{
    public InvalidFileSignatureException()
        : base("The file signature does not match the expected format.") { }


    public InvalidFileSignatureException(string fileName)
        : base($"The file '{fileName}' has an invalid or mismatched file signature.") { }


    public InvalidFileSignatureException(string fileName, Exception innerException)
        : base($"The file '{fileName}' has an invalid or mismatched file signature.", innerException) { }
}
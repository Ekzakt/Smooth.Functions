namespace Smooth.Functions.Infrastructure.Exceptions;

[Serializable]
public class FileExtensionNotInSignaturesListException : Exception
{
    public FileExtensionNotInSignaturesListException()
        : base("The file extension is not in the list of file signatures.") { }


    public FileExtensionNotInSignaturesListException(string? fileExtension)
        : base($"The file extension {fileExtension} is not in the list of file signatures.") { }


    public FileExtensionNotInSignaturesListException(string fileExtension, Exception innerException)
        : base($"The file extension {fileExtension} is not in the list of file signatures.", innerException) { }
}
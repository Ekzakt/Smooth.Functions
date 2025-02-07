namespace Smooth.Functions.Application.Configuration;

#nullable disable

public class FunctionOptions
{
    public const string SECTION_NAME = "";

    public string SmoothShopApiUrl { get; set; }

    public string SourceContainerName { get; set; }

    public string TargetContainerName { get; set; }

    public string BlobContainerMediaUploaded { get; set; }

    public string BlobContainerMediaProcessing { get; set; }

}

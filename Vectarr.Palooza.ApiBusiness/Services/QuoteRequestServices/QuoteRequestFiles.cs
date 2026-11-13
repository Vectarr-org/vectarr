namespace Vectarr.Palooza.ApiBusiness.Services.QuoteRequestServices
{
    /// <summary>
    /// A file received by the Create endpoint (decoupled from ASP.NET's IFormFile).
    /// </summary>
    public sealed class QuoteRequestUploadFile
    {
        public QuoteRequestUploadFile(string fileName, long length, Stream content)
        {
            FileName = fileName;
            Length = length;
            Content = content;
        }

        public string FileName { get; }
        public long Length { get; }
        public Stream Content { get; }
    }

    /// <summary>
    /// A stored quote request file resolved for download.
    /// </summary>
    public sealed class QuoteRequestDownload
    {
        public QuoteRequestDownload(string physicalPath, string fileName, string contentType)
        {
            PhysicalPath = physicalPath;
            FileName = fileName;
            ContentType = contentType;
        }

        public string PhysicalPath { get; }
        public string FileName { get; }
        public string ContentType { get; }
    }
}

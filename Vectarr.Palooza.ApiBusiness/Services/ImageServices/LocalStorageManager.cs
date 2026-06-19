using Vectarr.Palooza.Classes;
using Vectarr.Palooza.Exceptions;
using Microsoft.Extensions.Configuration;
using Vectarr.Palooza.ApiBusiness.IServices;

public class LocalStorageManager : IFileManager
{
    private readonly string _basePath;
    private readonly IConfiguration _configuration;

    public LocalStorageManager(IConfiguration configuration)
    {
        _configuration = configuration;
        _basePath = configuration["FileStorage:BasePath"];

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<Vectarr.Palooza.Classes.Api.File> UploadFile(
        Stream file,
        string filename,
        string directoryName,
        string webRootPathout,
        string contentType = null)
    {
        try
        {
            if (file.CanSeek)
            {
                file.Seek(0, SeekOrigin.Begin);
            }

            var filePath = Path.Combine(_basePath, filename);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            // Get base URL from config
            var baseUrl = _configuration["FileStorage:BaseUrl"];

            return new Vectarr.Palooza.Classes.Api.File()
            {
                Path = filename,
                StorageUrl = baseUrl  // Add this!
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error uploading file: {e.Message}");
            throw new ErrorValidationException("Error uploading file.");
        }
    }
}
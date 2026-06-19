namespace Vectarr.Palooza.ApiBusiness.IServices
{
    public interface IFileManager
    {
        Task<Classes.Api.File> UploadFile(Stream file, string filename, string directoryName, string webRootPathout, string contentType = null);
    }
}

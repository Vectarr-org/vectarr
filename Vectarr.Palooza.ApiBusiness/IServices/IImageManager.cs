using Vectarr.Palooza.Classes.Api;

namespace Vectarr.Palooza.ApiBusiness.IServices
{
    public interface IImageManager
    {
        Task<ImageModel> UploadImage(Stream image, string originalImageName, string webRootDirectory, string alt = null);
        void SaveImageAlt(int imageId, string alt);
    }
}

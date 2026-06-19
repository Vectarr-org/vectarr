using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.ApiBusiness.IServices;

namespace Vectarr.Palooza.ApiBusiness.Services.ImageServices
{
    public class ImageManager : IImageManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _storageManager;

        public ImageManager(IUnitOfWork unitOfWork, IFileManager storageManager)
        {
            _unitOfWork = unitOfWork;
            _storageManager = storageManager;
        }

        private string GenerateImageName(string filename)
        {
            var extension = Path.GetExtension(filename);
            var simpleName = Path.GetFileNameWithoutExtension(filename);
            var fullFileName = string.Format("{0}-{1}{2}", SEO.ToSeoUrl(simpleName), DateTime.UtcNow.Ticks, extension);
            return fullFileName;
        }

        public async Task<ImageModel> UploadImage(Stream image, string originalImageName, string webRootDirectory, string alt = null)
        {
            try
            {
                var url = await _storageManager.UploadFile(image, GenerateImageName(originalImageName), "profile-pictures", webRootDirectory, GetContentType(originalImageName));
                var imageModel = new ImageModel
                {
                    AltText = alt ?? "",
                    RelativeOriginalPath = url.Path,
                    BaseStorageUrl = url.StorageUrl,
                    RelativeThumbnailOriginalPath = ""
                };

                var newUnitOfWork = new UnitOfWork();
                return newUnitOfWork.Repositories.ImageRepository.Insert(imageModel);
            }
            catch (Exception ex)
            {
                throw new ErrorValidationException("Error: " + ex.Message);
            }
        }


        private string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).TrimStart('.').ToLower();

            switch (extension)
            {
                case "bmp":
                    return "image/bmp";
                case "png":
                    return "image/png";
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                default:
                    return "application/octet-stream";
            }
        }

        public ImageModel GetById(int? imageId)
        {
            return _unitOfWork.Repositories.ImageRepository.GetById(imageId.Value);
        }


        public void SaveImageAlt(int imageId, string alt)
        {
            var image = _unitOfWork.Repositories.ImageRepository.GetById(imageId);
            image.AltText = alt;
            _unitOfWork.Repositories.ImageRepository.Update(image);
        }
    }
}

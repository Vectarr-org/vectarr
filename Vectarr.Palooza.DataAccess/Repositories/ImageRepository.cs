using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Model;
using System.Linq.Expressions;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;


namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class ImageRepository : BaseRepository<Image, ImageModel>, IImageRepository
    {
        public ImageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<Image, ImageModel>> GetEntityToModelExpression()
        {
            return entity => entity != null ? new ImageModel
            {
                Id = entity.Id,
                RelativeOriginalPath = entity.OriginalPath,
                AltText = entity.AltText,
                RelativeThumbnailOriginalPath = entity.RelativeThumbnailOriginalPath,
                BaseStorageUrl = entity.StorageUrl,
                DateCreated = entity.CreatedDate
            } : null;
        }

        protected override Image ModelToEntityInternal(ImageModel model, Image entity)
        {

            entity.AltText = model.AltText;
            entity.OriginalPath = model.RelativeOriginalPath;
            entity.RelativeThumbnailOriginalPath = model.RelativeThumbnailOriginalPath;
            entity.StorageUrl = model.BaseStorageUrl;

            return entity;
        }
    }
}

using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Privacy;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.Services;

namespace Vectarr.Palooza.ApiBusiness.Services.UserServices
{
    public class IdentityServices : BaseManager<IdentityModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public IdentityServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.IdentityRepository)
        {
            _unitOfWork = unitOfWork;
        }

        protected override List<IdentityModel> LoadAdditionalInfo(List<IdentityModel> data)
        {
            var viewer = MarketplaceViewerContext.Current(_unitOfWork);
            foreach (var identity in data)
            {
                MarketplacePrivacy.Apply(identity, viewer);
            }

            return data;
        }

        protected override IdentityModel InsertInternal(IdentityModel item)
        {
            return Shape(base.InsertInternal(item), MarketplaceViewerContext.Current(_unitOfWork));
        }

        protected override IdentityModel UpdateInternal(IdentityModel item)
        {
            return Shape(base.UpdateInternal(item), MarketplaceViewerContext.Current(_unitOfWork));
        }

        protected override IdentityModel UpdateIncludeInternal(IdentityModel item)
        {
            return Shape(base.UpdateIncludeInternal(item), MarketplaceViewerContext.Current(_unitOfWork));
        }

        private static IdentityModel Shape(IdentityModel model, MarketplaceViewer viewer)
        {
            MarketplacePrivacy.Apply(model, viewer);
            return model;
        }
    }
}

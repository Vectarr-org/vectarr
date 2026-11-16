using Vectarr.Palooza.Classes.Privacy;

namespace Vectarr.Palooza.DataAccess
{
    public static class MarketplaceViewerContext
    {
        public static MarketplaceViewer Current(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                return MarketplaceViewer.Unknown();
            }

            var isAdmin = (unitOfWork.Identity?.IsVectarrAdmin ?? false) || unitOfWork.HasMarketplaceAdminVisibility;
            return MarketplaceViewer.Resolve(
                unitOfWork.RoleWasResolved,
                unitOfWork.Role,
                isAdmin,
                unitOfWork.Workshop?.Id ?? unitOfWork.WorkshopIdBeingLoaded,
                unitOfWork.Creator?.Id,
                unitOfWork.Identity?.Id);
        }
    }
}

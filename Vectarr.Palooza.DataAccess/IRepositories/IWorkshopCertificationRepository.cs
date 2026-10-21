using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IWorkshopCertificationRepository : IEntityRepository<WorkshopCertification, WorkshopCertificationModel>
    {
        List<CertificationReviewItemModel> ListForReview(
            CertificationReviewStatusEnum reviewStatus,
            int pageNumber,
            int countPerPage,
            string? orderBy,
            bool orderByDescending,
            out int totalCount);

        bool AnyVisible();
    }
}

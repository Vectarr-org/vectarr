using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopCertificationRepository : BaseRepository<WorkshopCertification, WorkshopCertificationModel>, IWorkshopCertificationRepository
    {
        public WorkshopCertificationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<WorkshopCertification, WorkshopCertificationModel>> GetEntityToModelExpression()
        {
            var imageExpression = UnitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            return entity => new WorkshopCertificationModel()
            {
                Id = entity.Id,
                Certification = entity.Certification,
                OtherCertification = entity.OtherCertification,
                DateCreated = entity.CreatedDate,
                LastModified = entity.LastModified,
                DocumentId = entity.DocumentId,
                Document = entity.Document != null ? imageExpression.Invoke(entity.Document) : null,
                ReviewStatus = entity.ReviewStatus,
                ReviewedByUserId = entity.ReviewedByUserId,
                ReviewedByEmail = entity.ReviewedBy != null ? entity.ReviewedBy.Email : null,
                ReviewedAt = entity.ReviewedAt,
                ExpiryDate = entity.ExpiryDate,
                RejectionReason = entity.RejectionReason,
            };
        }

        public List<CertificationReviewItemModel> ListForReview(
            CertificationReviewStatusEnum reviewStatus,
            int pageNumber,
            int countPerPage,
            string? orderBy,
            bool orderByDescending,
            out int totalCount)
        {
            // Same page bounds and skip as CertificationReviewQueue.Page: (pageNumber - 1) * countPerPage.
            CertificationReviewQueue.EnsurePage(pageNumber, countPerPage);
            var query = GetEntitiesQueryable(skipApplicationCheck: true)
                .Where(entity => !entity.IsHidden
                    && entity.ReviewStatus == reviewStatus
                    && !entity.Workshop.IsRemoved
                    && !entity.Workshop.IsHidden);

            query = ApplyPaging(query, pageNumber, countPerPage, out totalCount, orderBy, orderByDescending);
            return query.Select(GetReviewItemExpression()).ToList();
        }

        public bool AnyVisible()
        {
            return GetEntitiesQueryable(skipApplicationCheck: true).Any(entity => !entity.IsHidden);
        }

        private static Expression<Func<WorkshopCertification, CertificationReviewItemModel>> GetReviewItemExpression()
        {
            return entity => new CertificationReviewItemModel
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                LastModified = entity.LastModified,
                WorkshopId = entity.WorkshopId,
                WorkshopName = entity.Workshop.Name,
                Country = entity.Workshop.Country ?? (entity.Workshop.Address != null ? entity.Workshop.Address.Country : null),
                Certification = entity.Certification,
                OtherCertification = entity.OtherCertification,
                DocumentId = entity.DocumentId,
                ExpiryDate = entity.ExpiryDate,
                ReviewStatus = entity.ReviewStatus,
                ReviewedByUserId = entity.ReviewedByUserId,
                ReviewedByEmail = entity.ReviewedBy != null ? entity.ReviewedBy.Email : null,
                ReviewedAt = entity.ReviewedAt,
                RejectionReason = entity.RejectionReason,
            };
        }

        protected override WorkshopCertification ModelToEntityInternal(WorkshopCertificationModel model, WorkshopCertification entity)
        {
            var typeChanged = entity.Id != 0
                && (entity.Certification != model.Certification || entity.OtherCertification != model.OtherCertification);

            entity.Certification = model.Certification;
            entity.OtherCertification = model.OtherCertification;

            if (model.ExpiryDate.HasValue)
            {
                entity.ExpiryDate = model.ExpiryDate;
            }

            int? requestedDocumentId = null;
            if (model.Document != null && model.Document.Id > 0)
            {
                requestedDocumentId = model.Document.Id;
            }
            else if (model.DocumentId.HasValue && model.DocumentId.Value > 0)
            {
                requestedDocumentId = model.DocumentId;
            }

            var documentChanged = false;
            if (requestedDocumentId.HasValue)
            {
                documentChanged = entity.Id != 0 && entity.DocumentId != requestedDocumentId;
                entity.DocumentId = requestedDocumentId;
            }

            // Review fields are server-owned. A new certificate, a changed type, or a replaced file goes back to pending.
            if (entity.Id == 0 || typeChanged || documentChanged)
            {
                entity.ReviewStatus = CertificationReviewStatusEnum.Pending;
                entity.ReviewedByUserId = null;
                entity.ReviewedAt = null;
                entity.RejectionReason = null;
            }

            return entity;
        }
    }
}

using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Classes.Api;
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

using Vectarr.Palooza.DataAccess.Repositories;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class RepositoriesContainer : IRepositoriesContainer
    {
        internal RepositoriesContainer(IUnitOfWork unitOfWork)
        {
            ApplicationRepository = new ApplicationRepository(unitOfWork);
            UserAccountRepository = new UserAccountRepository(unitOfWork);
            IdentityRepository = new IdentityRepository(unitOfWork);
            AddressRepository = new AddressRepository(unitOfWork);
            ImageRepository = new ImageRepository(unitOfWork);
            ApplicationSubscriptionRepository = new ApplicationSubscriptionRepository(unitOfWork);
            ReferralRepsitory = new ReferralRepsitory(unitOfWork);
            WorkshopCapabilityRepository = new WorkshopCapabilityRepository(unitOfWork);
            WorkshopEquipmentRepository = new WorkshopEquipmentRepository(unitOfWork);
            WorkshopSpecializationRepository = new WorkshopSpecializationRepository(unitOfWork);
            WorkshopCertificationRepository = new WorkshopCertificationRepository(unitOfWork);
            WorkshopRepository = new WorkshopRepository(unitOfWork);
            CreatorRepository = new CreatorRepository(unitOfWork);
            JobRepository = new JobRepository(unitOfWork);
            BidRepository = new BidRepository(unitOfWork);
            WorkOrderProgressStageRepository = new WorkOrderProgressStageRepository(unitOfWork);
            WorkOrderProgressUpdateRepository = new WorkOrderProgressUpdateRepository(unitOfWork);
            WorkOrderRepository = new WorkOrderRepository(unitOfWork);
            ChatMessageRepository = new ChatMessageRepository(unitOfWork);
            QuoteRevisionRepository = new QuoteRevisionRepository(unitOfWork);
            WorkOrderQualityCheckRequestRepository = new WorkOrderQualityCheckRequestRepository(unitOfWork);
            JobQualityCheckRequestRepository = new JobQualityCheckRequestRepository(unitOfWork);
            WorkOrderQualityCheckRejectionRepository = new WorkOrderQualityCheckRejectionRepository(unitOfWork);
            WorkshopCustomProgressMessageRepository = new WorkshopCustomProgressMessageRepository(unitOfWork);
            BlogRepository = new BlogRepository(unitOfWork);
            BlogSectionRepository = new BlogSectionRepository(unitOfWork);
            BlogKeywordRepository = new BlogKeywordRepository(unitOfWork);
        }
        public IUserAccountRepository UserAccountRepository { get; }
        public IAppRepository ApplicationRepository { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IAddressRepository AddressRepository { get; }
        public IImageRepository ImageRepository { get; }
        public IApplicationSubscriptionRepository ApplicationSubscriptionRepository { get; }
        public IReferralRepsitory ReferralRepsitory { get; }
        public IWorkshopRepository WorkshopRepository { get; }
        public IWorkshopCapabilityRepository WorkshopCapabilityRepository { get; }
        public IWorkshopSpecializationRepository WorkshopSpecializationRepository { get; }
        public IWorkshopEquipmentRepository WorkshopEquipmentRepository { get; }
        public IWorkshopCertificationRepository WorkshopCertificationRepository { get; }
        public ICreatorRepository CreatorRepository { get; }
        public IJobRepository JobRepository { get; }
        public IBidRepository BidRepository { get; }
        public IWorkOrderProgressStageRepository WorkOrderProgressStageRepository { get; }
        public IWorkOrderProgressUpdateRepository WorkOrderProgressUpdateRepository { get; }
        public IWorkOrderRepository WorkOrderRepository { get; }
        public IChatMessageRepository ChatMessageRepository { get; }
        public IQuoteRevisionRepository QuoteRevisionRepository { get; }
        public IWorkOrderQualityCheckRequestRepository WorkOrderQualityCheckRequestRepository { get; }
        public IJobQualityCheckRequestRepository JobQualityCheckRequestRepository { get; }
        public IWorkOrderQualityCheckRejectionRepository WorkOrderQualityCheckRejectionRepository { get; }
        public IWorkshopCustomProgressMessageRepository WorkshopCustomProgressMessageRepository { get; }
        public IBlogRepository BlogRepository { get; }
        public IBlogSectionRepository BlogSectionRepository { get; }
        public IBlogKeywordRepository BlogKeywordRepository { get; }
    }
}

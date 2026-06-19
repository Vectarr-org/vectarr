namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IRepositoriesContainer
    {
        IUserAccountRepository UserAccountRepository { get; }
        IAppRepository ApplicationRepository { get; }
        IIdentityRepository IdentityRepository { get; }
        IAddressRepository AddressRepository { get; }
        IImageRepository ImageRepository { get; }
        IApplicationSubscriptionRepository ApplicationSubscriptionRepository { get; }
        IReferralRepsitory ReferralRepsitory { get; }
        IWorkshopSpecializationRepository WorkshopSpecializationRepository { get; }
        IWorkshopEquipmentRepository WorkshopEquipmentRepository { get; }
        IWorkshopCertificationRepository WorkshopCertificationRepository { get; }
        IWorkshopCapabilityRepository WorkshopCapabilityRepository { get; }
        ICreatorRepository CreatorRepository { get; }
        IWorkshopRepository WorkshopRepository { get; }
        IJobRepository JobRepository { get; }
        IBidRepository BidRepository { get; }
        IWorkOrderProgressStageRepository WorkOrderProgressStageRepository { get; }
        IWorkOrderProgressUpdateRepository WorkOrderProgressUpdateRepository { get; }
        IWorkOrderRepository WorkOrderRepository { get; }
        IChatMessageRepository ChatMessageRepository { get; }
        IQuoteRevisionRepository QuoteRevisionRepository { get; }
        IWorkOrderQualityCheckRequestRepository WorkOrderQualityCheckRequestRepository { get; }
        IJobQualityCheckRequestRepository JobQualityCheckRequestRepository { get; }
        IWorkOrderQualityCheckRejectionRepository WorkOrderQualityCheckRejectionRepository { get; }
        IWorkshopCustomProgressMessageRepository WorkshopCustomProgressMessageRepository { get; }
        IBlogRepository BlogRepository { get; }
        IBlogSectionRepository BlogSectionRepository { get; }
        IBlogKeywordRepository BlogKeywordRepository { get; }
    }
}

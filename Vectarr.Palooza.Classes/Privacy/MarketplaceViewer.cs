using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Privacy
{
    public enum MarketplaceViewerKind
    {
        /// <summary>Role could not be resolved. Fail closed: no shop contact and no exact ship-to.</summary>
        Unknown,
        Buyer,
        Shop,
        Admin
    }

    /// <summary>
    /// Who is reading a marketplace response. Admin is a resolved Admin role,
    /// Identity.IsVectarrAdmin, or an address listed in VectarrAdmin:Emails.
    /// </summary>
    public sealed class MarketplaceViewer
    {
        public MarketplaceViewerKind Kind { get; init; }
        public int? WorkshopId { get; init; }
        public int? CreatorId { get; init; }
        public int? IdentityId { get; init; }

        public static MarketplaceViewer Buyer(int? identityId = null) => new()
        {
            Kind = MarketplaceViewerKind.Buyer,
            IdentityId = identityId
        };

        public static MarketplaceViewer Shop(int workshopId, int? identityId = null) => new()
        {
            Kind = MarketplaceViewerKind.Shop,
            WorkshopId = workshopId,
            IdentityId = identityId
        };

        public static MarketplaceViewer Admin(int? workshopId = null) => new()
        {
            Kind = MarketplaceViewerKind.Admin,
            WorkshopId = workshopId
        };

        public static MarketplaceViewer Unknown() => new()
        {
            Kind = MarketplaceViewerKind.Unknown
        };

        public static MarketplaceViewer Resolve(bool roleWasResolved, RolesEnum role, bool isAdmin, int? workshopId, int? creatorId, int? identityId)
        {
            if (isAdmin || (roleWasResolved && role == RolesEnum.Admin))
            {
                return new MarketplaceViewer
                {
                    Kind = MarketplaceViewerKind.Admin,
                    WorkshopId = workshopId,
                    CreatorId = creatorId,
                    IdentityId = identityId
                };
            }

            if (!roleWasResolved)
            {
                return new MarketplaceViewer
                {
                    Kind = MarketplaceViewerKind.Unknown,
                    WorkshopId = workshopId,
                    CreatorId = creatorId,
                    IdentityId = identityId
                };
            }

            var kind = role switch
            {
                RolesEnum.Workshop => MarketplaceViewerKind.Shop,
                RolesEnum.Creator => MarketplaceViewerKind.Buyer,
                _ => MarketplaceViewerKind.Unknown
            };

            return new MarketplaceViewer
            {
                Kind = kind,
                WorkshopId = workshopId,
                CreatorId = creatorId,
                IdentityId = identityId
            };
        }
    }
}

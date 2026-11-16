using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Privacy
{
    /// <summary>
    /// Shapes marketplace API models before they leave the server.
    /// This mutates the response model only. Stored addresses are not changed;
    /// checkout still persists the buyer's full ship-to for tax and shipping.
    /// </summary>
    public static class MarketplacePrivacy
    {
        public static bool CanSeeFullShop(MarketplaceViewer viewer, int shopId)
        {
            if (viewer.Kind == MarketplaceViewerKind.Admin)
            {
                return true;
            }

            return viewer.Kind == MarketplaceViewerKind.Shop
                && viewer.WorkshopId.HasValue
                && viewer.WorkshopId.Value == shopId;
        }

        /// <summary>
        /// Exact ship-to (street and ZIP) is visible to the buyer, to admins,
        /// and to the shop that won an accepted or paid order.
        /// </summary>
        public static bool CanSeeExactShipping(MarketplaceViewer viewer, int? awardedWorkshopId, bool orderAcceptedOrPaid)
        {
            if (viewer.Kind == MarketplaceViewerKind.Admin || viewer.Kind == MarketplaceViewerKind.Buyer)
            {
                return true;
            }

            return viewer.Kind == MarketplaceViewerKind.Shop
                && orderAcceptedOrPaid
                && awardedWorkshopId.HasValue
                && viewer.WorkshopId.HasValue
                && viewer.WorkshopId.Value == awardedWorkshopId.Value;
        }

        /// <summary>
        /// A work order exists only after the buyer accepts a bid. There is no separate
        /// paid flag on marketplace orders; acceptance is the gate. Unknown future statuses
        /// stay hidden until they are explicitly treated as accepted or paid.
        /// </summary>
        public static bool IsAcceptedOrPaid(WorkOrderStatusEnum status)
        {
            return status is WorkOrderStatusEnum.CreatorAccepted
                or WorkOrderStatusEnum.Active
                or WorkOrderStatusEnum.Completed
                or WorkOrderStatusEnum.Cancelled
                or WorkOrderStatusEnum.Disputed;
        }

        public static void RedactExactShipping(AddressModel? address, bool canSeeExact)
        {
            if (address == null || canSeeExact)
            {
                return;
            }

            address.Street1 = null;
            address.Street2 = null;
            address.PostalCode = null;
        }

        public static void Apply(WorkshopModel? shop, MarketplaceViewer viewer)
        {
            if (shop == null || CanSeeFullShop(viewer, shop.Id))
            {
                return;
            }

            shop.PhoneNumber = null;
            shop.Email = null;
            if (shop.Address != null)
            {
                shop.Address.Street1 = null;
                shop.Address.Street2 = null;
                shop.Address.PostalCode = null;
            }
        }

        public static void Apply(JobModel? job, MarketplaceViewer viewer, bool includeBids = true)
        {
            if (job == null)
            {
                return;
            }

            var awarded = job.AwardedWorkshopId;
            var accepted = awarded.HasValue;
            RedactExactShipping(job.Creator?.Address, CanSeeExactShipping(viewer, awarded, accepted));

            if (includeBids && job.Bids != null)
            {
                foreach (var bid in job.Bids)
                {
                    Apply(bid, viewer, includeJob: false);
                }
            }
        }

        public static void Apply(BidModel? bid, MarketplaceViewer viewer, bool includeJob = true)
        {
            if (bid == null)
            {
                return;
            }

            Apply(bid.Workshop, viewer);
            if (includeJob)
            {
                Apply(bid.Job, viewer, includeBids: false);
            }
        }

        public static void Apply(WorkOrderModel? order, MarketplaceViewer viewer)
        {
            if (order == null)
            {
                return;
            }

            var accepted = IsAcceptedOrPaid(order.Status);
            if (!CanSeeExactShipping(viewer, order.WorkshopId, accepted))
            {
                order.ShippingAddress = null;
            }

            Apply(order.AcceptedBid?.Workshop, viewer);
            if (order.Job != null)
            {
                if (!order.Job.AwardedWorkshopId.HasValue && accepted)
                {
                    order.Job.AwardedWorkshopId = order.WorkshopId;
                }

                Apply(order.Job, viewer);
            }
        }

        public static void Apply(IdentityModel? identity, MarketplaceViewer viewer)
        {
            if (identity == null || viewer.Kind == MarketplaceViewerKind.Admin)
            {
                return;
            }

            if (viewer.IdentityId.HasValue && identity.Id == viewer.IdentityId.Value)
            {
                return;
            }

            // Phone and personal email stay on the person's own record and on admin responses.
            // Exact ship-to is not served from the identity directory; the awarded shop reads it
            // from the accepted work order.
            identity.PhoneNumber = null;
            identity.EmailAddress = null;
            RedactExactShipping(identity.Address, canSeeExact: false);
        }

        /// <summary>
        /// Shop fields a buyer email may mention: name, city, and state. Never phone, email, street, or ZIP.
        /// </summary>
        public static string BuyerNotice(WorkshopModel? shop)
        {
            if (shop == null)
            {
                return string.Empty;
            }

            var copy = Clone(shop);
            Apply(copy, MarketplaceViewer.Buyer());
            var place = string.Join(", ", new[] { copy.Address?.City, copy.Address?.State }
                .Where(part => !string.IsNullOrWhiteSpace(part)));
            var name = string.IsNullOrWhiteSpace(copy.Name) ? "Shop" : copy.Name.Trim();
            return string.IsNullOrEmpty(place) ? name : $"{name} ({place})";
        }

        public static WorkshopModel Clone(WorkshopModel shop)
        {
            return new WorkshopModel
            {
                Id = shop.Id,
                Name = shop.Name,
                CompanyName = shop.CompanyName,
                WebsiteUrl = shop.WebsiteUrl,
                Description = shop.Description,
                PhoneNumber = shop.PhoneNumber,
                Email = shop.Email,
                Address = shop.Address == null
                    ? null
                    : new AddressModel
                    {
                        Id = shop.Address.Id,
                        Country = shop.Address.Country,
                        Street1 = shop.Address.Street1,
                        Street2 = shop.Address.Street2,
                        City = shop.Address.City,
                        State = shop.Address.State,
                        PostalCode = shop.Address.PostalCode
                    }
            };
        }
    }
}

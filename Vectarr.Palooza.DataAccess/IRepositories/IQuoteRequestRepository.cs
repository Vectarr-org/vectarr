using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IQuoteRequestRepository : IEntityRepository<QuoteRequest, QuoteRequestModel>
    {
        /// <summary>
        /// Loads one request as an API model (AccessToken and AdminNotes included; callers shape the response).
        /// The (potentially large) QuoteSnapshotJson is only loaded when includeSnapshot is true.
        /// </summary>
        QuoteRequestModel? GetModel(int id, bool includeSnapshot);

        /// <summary>
        /// Newest-first paged search. owner / status / search are optional filters.
        /// Owner filter: IdentityId == ownerIdentityId OR (IdentityId is null AND ContactEmail == ownerEmail, case-insensitive);
        /// pass ownerEmail = null to match on the identity only.
        /// Search matches reference, contact name/email, company, part name/number (case-insensitive).
        /// </summary>
        List<QuoteRequestModel> Search(int? ownerIdentityId, string? ownerEmail, QuoteRequestStatusEnum? status, string? search, int pageNumber, int countPerPage, out int totalCount);

        /// <summary>
        /// Links guest requests (IdentityId null) whose ContactEmail matches (case-insensitive) to the identity.
        /// Returns the number of linked requests.
        /// </summary>
        int LinkGuestRequests(int identityId, string email);

        bool ReferenceNumberExists(string referenceNumber);

        /// <summary>Saves changes made to a tracked entity (implemented by BaseRepository).</summary>
        void Update(QuoteRequest entity);

        /// <summary>
        /// Atomically records a successful payment if the request has not been marked paid yet.
        /// Returns true only for the call that actually performed the update (so notifications are sent once).
        /// </summary>
        bool TryMarkPaid(int id, decimal amountPaid, string? paymentIntentId, string? checkoutSessionId, DateTime paidAtUtc);

        QuoteRequestStatsModel GetStats();

        /// <summary>Payment totals of one owner's requests (same owner rule as Search).</summary>
        QuoteRequestPaymentSummaryModel GetPaymentSummary(int ownerIdentityId, string? ownerEmail);
    }
}

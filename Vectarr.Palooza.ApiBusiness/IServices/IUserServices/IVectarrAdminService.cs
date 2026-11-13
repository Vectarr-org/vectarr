using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.IServices.IUserServices
{
    /// <summary>
    /// Single place that decides whether a user is a Vectarr admin.
    /// A user is a Vectarr admin when Identity.IsVectarrAdmin is true, or when their email
    /// (case-insensitive) is listed in the "VectarrAdmin:Emails" configuration array.
    /// </summary>
    public interface IVectarrAdminService
    {
        /// <summary>
        /// True when the currently authenticated user (IUnitOfWork.CurrentUser) is a Vectarr admin.
        /// </summary>
        bool IsCurrentUserVectarrAdmin();

        /// <summary>
        /// True when the given identity / account email belongs to a Vectarr admin.
        /// </summary>
        bool IsVectarrAdmin(Identity? identity, string? accountEmail = null);
    }
}

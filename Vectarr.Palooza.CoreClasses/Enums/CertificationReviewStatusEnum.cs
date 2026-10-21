namespace Vectarr.Palooza.CoreClasses.Enums
{
    /// <summary>
    /// Review state for a shop certificate upload. New uploads start at <see cref="Pending"/>.
    /// </summary>
    public enum CertificationReviewStatusEnum
    {
        Pending = 0,
        Verified = 1,
        Rejected = 2
    }
}

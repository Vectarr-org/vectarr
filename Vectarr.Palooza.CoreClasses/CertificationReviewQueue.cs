using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses
{
    /// <summary>
    /// Rules for the admin certificate review list.
    /// An omitted status is Pending. Paging matches the existing list endpoints:
    /// page numbers start at 1, and total pages is the ceiling of count / countPerPage.
    /// </summary>
    public static class CertificationReviewQueue
    {
        public static CertificationReviewStatusEnum ResolveStatus(CertificationReviewStatusEnum? requested)
        {
            if (!requested.HasValue)
            {
                return CertificationReviewStatusEnum.Pending;
            }

            if (!Enum.IsDefined(requested.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(requested), "Review status is not recognized.");
            }

            return requested.Value;
        }

        public static void EnsurePage(int pageNumber, int countPerPage)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "The page number should be 1 or higher");
            }

            if (countPerPage < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(countPerPage), "The page size should be 1 or higher");
            }
        }

        public static int TotalPages(int totalCount, int countPerPage)
        {
            EnsurePage(1, countPerPage);
            return (int)Math.Ceiling((decimal)totalCount / countPerPage);
        }

        /// <summary>
        /// Filters to the resolved review status, preserving order, then applies the list skip/take.
        /// The repository query uses the same status and the same skip of (pageNumber - 1) * countPerPage.
        /// </summary>
        public static List<T> Page<T>(
            IEnumerable<T> items,
            Func<T, CertificationReviewStatusEnum> status,
            CertificationReviewStatusEnum? requested,
            int pageNumber,
            int countPerPage,
            out int totalCount)
        {
            EnsurePage(pageNumber, countPerPage);
            var resolved = ResolveStatus(requested);
            var filtered = items.Where(item => status(item) == resolved).ToList();
            totalCount = filtered.Count;
            var skip = (pageNumber - 1) * countPerPage;
            return filtered.Skip(skip).Take(countPerPage).ToList();
        }
    }
}

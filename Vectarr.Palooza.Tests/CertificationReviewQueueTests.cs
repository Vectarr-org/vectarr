using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;
using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class CertificationReviewQueueTests
    {
        private sealed record Row(int Id, CertificationReviewStatusEnum Status);

        [Fact]
        public void OmittedStatus_DefaultsToPending_AndPagesLikeTheListEndpoints()
        {
            var rows = new[]
            {
                new Row(1, CertificationReviewStatusEnum.Pending),
                new Row(2, CertificationReviewStatusEnum.Verified),
                new Row(3, CertificationReviewStatusEnum.Pending),
                new Row(4, CertificationReviewStatusEnum.Rejected),
                new Row(5, CertificationReviewStatusEnum.Pending),
            };

            Assert.Equal(CertificationReviewStatusEnum.Pending, CertificationReviewQueue.ResolveStatus(null));

            var firstPage = CertificationReviewQueue.Page(
                rows,
                row => row.Status,
                requested: null,
                pageNumber: 1,
                countPerPage: 2,
                out var totalCount);

            Assert.Equal(3, totalCount);
            Assert.Equal(2, CertificationReviewQueue.TotalPages(totalCount, 2));
            Assert.Equal(new[] { 1, 3 }, firstPage.Select(row => row.Id));

            var secondPage = CertificationReviewQueue.Page(
                rows,
                row => row.Status,
                requested: null,
                pageNumber: 2,
                countPerPage: 2,
                out _);
            Assert.Equal(new[] { 5 }, secondPage.Select(row => row.Id));
        }

        [Fact]
        public void ExplicitStatus_ReturnsOnlyThatStatus()
        {
            var rows = new[]
            {
                new Row(1, CertificationReviewStatusEnum.Pending),
                new Row(2, CertificationReviewStatusEnum.Verified),
                new Row(3, CertificationReviewStatusEnum.Rejected),
            };

            var verified = CertificationReviewQueue.Page(
                rows,
                row => row.Status,
                CertificationReviewStatusEnum.Verified,
                pageNumber: 1,
                countPerPage: 10,
                out var verifiedCount);
            var rejected = CertificationReviewQueue.Page(
                rows,
                row => row.Status,
                CertificationReviewStatusEnum.Rejected,
                pageNumber: 1,
                countPerPage: 10,
                out var rejectedCount);

            Assert.Equal(new[] { 2 }, verified.Select(row => row.Id));
            Assert.Equal(1, verifiedCount);
            Assert.Equal(new[] { 3 }, rejected.Select(row => row.Id));
            Assert.Equal(1, rejectedCount);
            Assert.Equal(0, CertificationReviewQueue.TotalPages(0, 10));
        }

        [Fact]
        public void InvalidStatusOrPage_IsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CertificationReviewQueue.ResolveStatus((CertificationReviewStatusEnum)99));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CertificationReviewQueue.EnsurePage(pageNumber: 0, countPerPage: 10));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CertificationReviewQueue.EnsurePage(pageNumber: 1, countPerPage: 0));
        }
    }
}

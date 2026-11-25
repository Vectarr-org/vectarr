using Vectarr.Palooza.Classes.Metrics;
using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class SiteFunnelBucketsTests
    {
        [Theory]
        [InlineData(null, "none")]
        [InlineData("", "none")]
        [InlineData("   ", "none")]
        [InlineData("x.com", "x.com")]
        [InlineData("www.x.com", "x.com")]
        [InlineData("https://x.com/vectarrr/status/1", "x.com")]
        [InlineData("https://www.x.com/", "x.com")]
        [InlineData("twitter.com", "x.com")]
        [InlineData("https://twitter.com/share", "x.com")]
        [InlineData("t.co", "t.co")]
        [InlineData("https://t.co/abc", "t.co")]
        [InlineData("linkedin.com", "linkedin.com")]
        [InlineData("www.linkedin.com", "linkedin.com")]
        [InlineData("https://www.linkedin.com/feed/", "linkedin.com")]
        [InlineData("lnkd.in", "linkedin.com")]
        [InlineData("https://lnkd.in/abc", "linkedin.com")]
        [InlineData("google.com", "other")]
        [InlineData("https://news.ycombinator.com/item?id=1", "other")]
        [InlineData("https://vectarr.com/rfq-checklist", "other")]
        public void MapReferrerBucketsAllowlistedHostsOnly(string? input, string expected)
        {
            Assert.Equal(expected, SiteFunnelBuckets.MapReferrer(input));
        }

        [Fact]
        public void MapReferrerNeverReturnsTheOriginalHostForOther()
        {
            var mapped = SiteFunnelBuckets.MapReferrer("https://secret.example/path?q=1");
            Assert.Equal("other", mapped);
            Assert.DoesNotContain("secret", mapped, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("example", mapped, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("path", mapped, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void VisitHitStoresOnlyTheBucketName()
        {
            Assert.True(SiteFunnelBuckets.TryResolveHit("visit", "https://www.linkedin.com/in/someone", out var bucket, out var error));
            Assert.Equal("visit:linkedin.com", bucket);
            Assert.Equal(string.Empty, error);
        }

        [Fact]
        public void QuotesStartedIgnoresReferrerAndDoesNotKeepIt()
        {
            Assert.True(SiteFunnelBuckets.TryResolveHit("quotes_started", "https://example.com/do-not-store", out var bucket, out _));
            Assert.Equal("quotes_started", bucket);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("pageview")]
        [InlineData("VISIT ")]
        [InlineData("click")]
        public void UnknownOrMissingEventsAreRejected(string? eventName)
        {
            if (eventName == "VISIT ")
            {
                // Trim + case-insensitive: this one is allowed.
                Assert.True(SiteFunnelBuckets.TryResolveHit(eventName, null, out var bucket, out _));
                Assert.Equal("visit:none", bucket);
                return;
            }

            Assert.False(SiteFunnelBuckets.TryResolveHit(eventName, "x.com", out var rejected, out var error));
            Assert.Equal(string.Empty, rejected);
            Assert.False(string.IsNullOrEmpty(error));
        }

        [Fact]
        public void VisitWithNoReferrerCountsAsNone()
        {
            Assert.True(SiteFunnelBuckets.TryResolveHit("visit", null, out var bucket, out _));
            Assert.Equal("visit:none", bucket);
        }

        [Fact]
        public void OversizedReferrerIsRejected()
        {
            var tooLong = new string('a', SiteFunnelBuckets.MaxReferrerChars + 1) + ".com";
            Assert.False(SiteFunnelBuckets.TryResolveHit("visit", tooLong, out var bucket, out var error));
            Assert.Equal(string.Empty, bucket);
            Assert.Contains("too long", error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReferrerWithSpacesIsRejected()
        {
            Assert.False(SiteFunnelBuckets.TryResolveHit("visit", "x.com extra", out var bucket, out _));
            Assert.Equal(string.Empty, bucket);
        }

        [Fact]
        public void TodayInCentralUsesAmericaChicago()
        {
            // 2026-11-25 04:30 UTC is still 2026-11-24 22:30 in Chicago (CST, UTC-6).
            var justBeforeMidnight = new DateTimeOffset(2026, 11, 25, 4, 30, 0, TimeSpan.Zero);
            Assert.Equal(new DateOnly(2026, 11, 24), SiteFunnelBuckets.TodayInCentral(justBeforeMidnight));

            // 2026-11-25 06:30 UTC is 2026-11-25 00:30 in Chicago.
            var justAfterMidnight = new DateTimeOffset(2026, 11, 25, 6, 30, 0, TimeSpan.Zero);
            Assert.Equal(new DateOnly(2026, 11, 25), SiteFunnelBuckets.TodayInCentral(justAfterMidnight));
        }

        [Fact]
        public void AggregateRollsXAndTcoTogetherAndIgnoresUnknownBuckets()
        {
            var day = new DateOnly(2026, 11, 24);
            var rows = SiteFunnelBuckets.Aggregate(
            [
                new SiteFunnelCount { Date = day, Bucket = "visit:x.com", Count = 3 },
                new SiteFunnelCount { Date = day, Bucket = "visit:t.co", Count = 2 },
                new SiteFunnelCount { Date = day, Bucket = "visit:linkedin.com", Count = 4 },
                new SiteFunnelCount { Date = day, Bucket = "visit:other", Count = 1 },
                new SiteFunnelCount { Date = day, Bucket = "visit:none", Count = 6 },
                new SiteFunnelCount { Date = day, Bucket = "quotes_started", Count = 5 },
                new SiteFunnelCount { Date = day, Bucket = "visit:secret.example", Count = 99 },
                new SiteFunnelCount { Date = day, Bucket = "not-allowlisted", Count = 7 },
            ]);

            var only = Assert.Single(rows);
            Assert.Equal("2026-11-24", only.Date);
            Assert.Equal(3 + 2 + 4 + 1 + 6, only.Visits);
            Assert.Equal(5, only.XReferrals);
            Assert.Equal(4, only.LinkedInReferrals);
            Assert.Equal(5, only.QuotesStarted);
        }
    }
}

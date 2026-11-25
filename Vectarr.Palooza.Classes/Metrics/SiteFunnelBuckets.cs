using System.Globalization;
using System.Text.Json.Serialization;

namespace Vectarr.Palooza.Classes.Metrics
{
    /// <summary>
    /// Maps a referrer host to an allowlisted bucket and aggregates daily rows
    /// into the funnel CSV shape. Never keeps the original URL or an unknown host.
    /// </summary>
    public static class SiteFunnelBuckets
    {
        public const string TimeZoneId = "America/Chicago";
        public const string EventVisit = "visit";
        public const string EventQuotesStarted = "quotes_started";
        public const int MaxBodyBytes = 1024;
        public const int MaxReferrerChars = 253;

        public const string ReferrerX = "x.com";
        public const string ReferrerTco = "t.co";
        public const string ReferrerLinkedIn = "linkedin.com";
        public const string ReferrerOther = "other";
        public const string ReferrerNone = "none";

        public const string QuotesStartedBucket = EventQuotesStarted;

        public static readonly IReadOnlyList<string> VisitReferrerBuckets =
        [
            ReferrerX,
            ReferrerTco,
            ReferrerLinkedIn,
            ReferrerOther,
            ReferrerNone
        ];

        public static string VisitBucketName(string referrerBucket) => "visit:" + referrerBucket;

        public static DateOnly TodayInCentral(DateTimeOffset? utcNow = null)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var instant = utcNow ?? DateTimeOffset.UtcNow;
            var local = TimeZoneInfo.ConvertTime(instant, tz);
            return DateOnly.FromDateTime(local.DateTime);
        }

        /// <summary>
        /// Turns a hostname (or, if a full URL was sent by mistake, its host)
        /// into one of the visit referrer buckets. The original string is not returned.
        /// </summary>
        public static string MapReferrer(string? referrer)
        {
            var host = ExtractHost(referrer);
            if (string.IsNullOrEmpty(host))
            {
                return ReferrerNone;
            }

            if (host is "x.com" or "www.x.com" or "twitter.com" or "www.twitter.com")
            {
                return ReferrerX;
            }

            if (host is "t.co" or "www.t.co")
            {
                return ReferrerTco;
            }

            if (IsLinkedInHost(host))
            {
                return ReferrerLinkedIn;
            }

            return ReferrerOther;
        }

        public static bool TryResolveHit(string? eventName, string? referrer, out string bucket, out string error)
        {
            bucket = string.Empty;
            error = string.Empty;

            var ev = (eventName ?? string.Empty).Trim();
            if (ev.Length == 0)
            {
                error = "event is required";
                return false;
            }

            if (ev.Equals(EventQuotesStarted, StringComparison.OrdinalIgnoreCase))
            {
                bucket = QuotesStartedBucket;
                return true;
            }

            if (!ev.Equals(EventVisit, StringComparison.OrdinalIgnoreCase))
            {
                error = "event is not allowlisted";
                return false;
            }

            if (referrer != null && referrer.Length > MaxReferrerChars)
            {
                error = "referrer is too long";
                return false;
            }

            if (!TryExtractHost(referrer, out _, out var hostError))
            {
                error = hostError;
                return false;
            }

            var mapped = MapReferrer(referrer);
            if (!VisitReferrerBuckets.Contains(mapped, StringComparer.Ordinal))
            {
                error = "referrer is not allowlisted";
                return false;
            }

            bucket = VisitBucketName(mapped);
            return true;
        }

        public static IReadOnlyList<SiteFunnelDailyRow> Aggregate(IEnumerable<SiteFunnelCount> counts)
        {
            var byDate = new SortedDictionary<DateOnly, SiteFunnelDailyRow>();
            foreach (var row in counts)
            {
                if (!byDate.TryGetValue(row.Date, out var day))
                {
                    day = new SiteFunnelDailyRow
                    {
                        Date = row.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    };
                    byDate[row.Date] = day;
                }

                if (row.Bucket == QuotesStartedBucket)
                {
                    day.QuotesStarted += row.Count;
                    continue;
                }

                const string visitPrefix = "visit:";
                if (!row.Bucket.StartsWith(visitPrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var referrer = row.Bucket[visitPrefix.Length..];
                if (!VisitReferrerBuckets.Contains(referrer, StringComparer.Ordinal))
                {
                    continue;
                }

                day.Visits += row.Count;
                if (referrer is ReferrerX or ReferrerTco)
                {
                    day.XReferrals += row.Count;
                }
                else if (referrer is ReferrerLinkedIn)
                {
                    day.LinkedInReferrals += row.Count;
                }
            }

            return byDate.Values.ToList();
        }

        public static string ExtractHost(string? referrer)
        {
            TryExtractHost(referrer, out var host, out _);
            return host;
        }

        /// <summary>
        /// Returns false only when the input is present but cannot be treated as a
        /// host or URL. Blank input is valid and means "none".
        /// </summary>
        public static bool TryExtractHost(string? referrer, out string hostOut, out string error)
        {
            hostOut = string.Empty;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(referrer))
            {
                return true;
            }

            var raw = referrer.Trim();
            if (raw.Length > MaxReferrerChars)
            {
                error = "referrer is too long";
                return false;
            }

            if (raw.Any(char.IsWhiteSpace) || raw.Any(c => char.IsControl(c)))
            {
                error = "referrer is not a host";
                return false;
            }

            string host;
            if (raw.Contains("://", StringComparison.Ordinal) || raw.Contains('/', StringComparison.Ordinal) || raw.Contains('?', StringComparison.Ordinal))
            {
                var candidate = raw;
                if (!candidate.Contains("://", StringComparison.Ordinal))
                {
                    candidate = "http://" + candidate;
                }

                if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri) || string.IsNullOrEmpty(uri.Host))
                {
                    error = "referrer is not a host";
                    return false;
                }

                host = uri.Host;
            }
            else
            {
                host = raw;
                if (host.Contains(':', StringComparison.Ordinal))
                {
                    // hostname:port
                    var parts = host.Split(':');
                    if (parts.Length != 2 || !int.TryParse(parts[1], out _))
                    {
                        error = "referrer is not a host";
                        return false;
                    }
                    host = parts[0];
                }
            }

            host = host.Trim().Trim('.').ToLowerInvariant();
            if (host.Length == 0 || host.Contains('/') || host.Contains('?') || host.Contains('#') || host.Contains('@'))
            {
                error = "referrer is not a host";
                return false;
            }

            hostOut = host;
            return true;
        }

        private static bool IsLinkedInHost(string host)
        {
            return host == "linkedin.com"
                || host.EndsWith(".linkedin.com", StringComparison.Ordinal)
                || host == "lnkd.in"
                || host.EndsWith(".lnkd.in", StringComparison.Ordinal);
        }
    }

    public sealed class SiteFunnelCount
    {
        public DateOnly Date { get; set; }
        public string Bucket { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public sealed class SiteFunnelHitRequest
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }
        [JsonPropertyName("referrer")]
        public string? Referrer { get; set; }
    }

    public sealed class SiteFunnelDailyRow
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;
        [JsonPropertyName("visits")]
        public int Visits { get; set; }
        [JsonPropertyName("x_referrals")]
        public int XReferrals { get; set; }
        [JsonPropertyName("linkedin_referrals")]
        public int LinkedInReferrals { get; set; }
        [JsonPropertyName("quotes_started")]
        public int QuotesStarted { get; set; }
    }

    public sealed class SiteFunnelDailyResponse
    {
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = SiteFunnelBuckets.TimeZoneId;
        [JsonPropertyName("days")]
        public IReadOnlyList<SiteFunnelDailyRow> Days { get; set; } = [];
    }
}

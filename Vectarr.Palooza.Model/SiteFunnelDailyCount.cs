using System.ComponentModel.DataAnnotations.Schema;

namespace Vectarr.Palooza.Model
{
    /// <summary>
    /// One day's aggregate count for a single funnel bucket.
    /// Stores only the date, the bucket name, and the count — no IP addresses,
    /// user agents, cookies, identifiers, or URLs.
    /// </summary>
    [Table("Metrics.DailyCounts")]
    public class SiteFunnelDailyCount
    {
        /// <summary>Calendar date in America/Chicago.</summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Allowlisted name only: visit:x.com, visit:t.co, visit:linkedin.com,
        /// visit:other, visit:none, or quotes_started.
        /// </summary>
        public string Bucket { get; set; } = null!;

        public int Count { get; set; }
    }
}

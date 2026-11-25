using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Classes.Metrics;
using Vectarr.Palooza.Context;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.Services.Metrics
{
    /// <summary>
    /// Increments and reads the cookie-free daily funnel counters.
    /// </summary>
    public class SiteFunnelService
    {
        private readonly VectarrPostgresDBContext _db;

        public SiteFunnelService(VectarrPostgresDBContext db)
        {
            _db = db;
        }

        public async Task IncrementAsync(string bucket, CancellationToken cancellationToken = default)
        {
            var date = SiteFunnelBuckets.TodayInCentral();
            // PostgreSQL upsert so two hits at the same instant both count.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"""
                INSERT INTO "Metrics.DailyCounts" ("Date", "Bucket", "Count")
                VALUES ({date}, {bucket}, 1)
                ON CONFLICT ("Date", "Bucket")
                DO UPDATE SET "Count" = "Metrics.DailyCounts"."Count" + 1
                """,
                cancellationToken);
        }

        public async Task<SiteFunnelDailyResponse> GetDailyAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        {
            var query = _db.SiteFunnelDailyCounts.AsNoTracking();
            if (from.HasValue)
            {
                query = query.Where(x => x.Date >= from.Value);
            }
            if (to.HasValue)
            {
                query = query.Where(x => x.Date <= to.Value);
            }

            var rows = await query
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Bucket)
                .Select(x => new SiteFunnelCount
                {
                    Date = x.Date,
                    Bucket = x.Bucket,
                    Count = x.Count
                })
                .ToListAsync(cancellationToken);

            return new SiteFunnelDailyResponse
            {
                Timezone = SiteFunnelBuckets.TimeZoneId,
                Days = SiteFunnelBuckets.Aggregate(rows)
            };
        }
    }
}

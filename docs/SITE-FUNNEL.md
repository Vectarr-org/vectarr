# Cookie-free site funnel counters

Daily aggregate counts of vectarr.com visits (by referring site) and instant quotes started. Used by the customer-targeting test. This is first-party, cookie-free counting.

## What is stored

Table `Metrics.DailyCounts` (composite key `Date` + `Bucket`):

- `Date` — calendar date in America/Chicago
- `Bucket` — one of `visit:x.com`, `visit:t.co`, `visit:linkedin.com`, `visit:other`, `visit:none`, `quotes_started`
- `Count` — integer

Nothing else is written for this feature: no IP addresses, user agents, cookies, user or session IDs, or full URLs. If a full URL is sent as the referrer, only the host is mapped to a bucket and the URL is discarded.

`twitter.com` is counted as `x.com`. LinkedIn short links (`lnkd.in` and `*.linkedin.com`) are counted as `linkedin.com`. The published CSV column `x_referrals` is `x.com` plus `t.co`.

## API

- `POST /api/metrics/hit` — anonymous. JSON body `{"event":"visit","referrer":"x.com"}` or `{"event":"quotes_started"}`. `referrer` must be a hostname (a full URL is accepted and reduced to its host). Bodies larger than 1,024 bytes are rejected. Success is `204`.
- `GET /api/metrics/daily?from=2026-11-25&to=2026-12-08` — returns `{ "timezone": "America/Chicago", "days": [ { "date", "visits", "x_referrals", "linkedin_referrals", "quotes_started" } ] }`. Auth: a Vectarr admin JWT, or the header `X-Metrics-Key` matching `SiteFunnel:ReadKey`. If that config value is empty, only an admin JWT works.

## Deploy

This change **requires a database migration**. After publishing the API:

1. Set `SiteFunnel:ReadKey` in the server's secret settings (not the repo `appsettings.json`). Use a long random string.
2. Apply the EF migration `AddSiteFunnelDailyCounts`, for example:

   ```bash
   dotnet ef database update --project Vectarr.Palooza.Context --startup-project Vectarr.Palooza.WebApi
   ```

   or run the migration SQL generated from that project against the production database. The previous release (`8c8f3c5`, shop fee tiers and privacy) had no migration; this one does.

3. Confirm `POST https://api.vectarr.com/api/metrics/hit` returns 204 for `{"event":"visit","referrer":"none"}`, and that `GET /api/metrics/daily` without a key or admin token returns 401.

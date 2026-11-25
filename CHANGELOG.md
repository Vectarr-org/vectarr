# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Cookie-free first-party funnel counters. `POST /api/metrics/hit` is anonymous and increments a daily aggregate only: page visits bucketed by referrer (`x.com`, `t.co`, `linkedin.com`, `other`, or `none`) and `quotes_started` when a CAD upload or ballpark begins. The table `Metrics.DailyCounts` stores only date, bucket name, and count — no IP addresses, user agents, cookies, identifiers, or URLs. Request bodies are capped at 1,024 bytes and unknown event or referrer values are rejected. `GET /api/metrics/daily` returns those daily totals (timezone America/Chicago) and is limited to Vectarr admins or a `SiteFunnel:ReadKey` sent as the `X-Metrics-Key` header. An empty or missing key never grants access. **This release adds an EF Core migration (`AddSiteFunnelDailyCounts`) that must be applied on deploy** with `dotnet ef database update` or by running the migration SQL. Set `SiteFunnel:ReadKey` in the server's secret settings file; do not put a real key in the repo `appsettings.json`.

- Shop platform fee tiers (config and calculator only; not charged or stored on an order yet) and marketplace contact redaction: buyers see a shop's city and state only, and bidding shops see a buyer's city and state only until an order is accepted. (#3; already on `main`, awaiting deploy)

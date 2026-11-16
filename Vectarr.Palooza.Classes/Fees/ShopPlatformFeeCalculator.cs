namespace Vectarr.Palooza.Classes.Fees
{
    public class ShopPlatformFeeTier
    {
        /// <summary>
        /// Exclusive upper bound of the order price, in the same currency as the order.
        /// A tier covers prices from the previous bound up to, but not including, this amount.
        /// Null means no upper bound (the top tier, 500k and over).
        /// </summary>
        public decimal? MaxExclusive { get; set; }

        public decimal Percent { get; set; }
    }

    /// <summary>
    /// Bound from the "ShopPlatformFee" configuration section.
    /// This is the shop's platform fee. It is not the instant-quote buyer deposit
    /// (InstantQuote:InitialFeePercent).
    /// </summary>
    public class ShopPlatformFeeOptions
    {
        public const string SectionName = "ShopPlatformFee";

        public List<ShopPlatformFeeTier> Tiers { get; set; } = ShopPlatformFeeCalculator.DefaultTiers();
    }

    public readonly record struct ShopPlatformFeeResult(decimal OrderPrice, decimal Percent, decimal Fee);

    /// <summary>
    /// Single calculator for the shop platform fee. Each tier runs from its listed
    /// amount up to, but not including, the next one (min &lt;= price &lt; next min).
    /// $999.99 is 10%. Exactly 1000 is 9%. Exactly 500000 is 1%.
    /// There is no 1.5% band. This matches the frontend table in Vectarr-org/machining#8.
    ///
    /// Nothing in the API charges this yet. Call <see cref="Calculate"/> when a shop payout
    /// is taken, using the completed order price (bid amount plus shipping, taxes, and
    /// tariffs when those are part of the price, or WorkOrder.CurrentPrice once that
    /// field is maintained). Do not use it for the instant-quote buyer deposit.
    /// </summary>
    public static class ShopPlatformFeeCalculator
    {
        public static List<ShopPlatformFeeTier> DefaultTiers() => new()
        {
            new ShopPlatformFeeTier { MaxExclusive = 1000m, Percent = 10m },
            new ShopPlatformFeeTier { MaxExclusive = 5000m, Percent = 9m },
            new ShopPlatformFeeTier { MaxExclusive = 10000m, Percent = 8m },
            new ShopPlatformFeeTier { MaxExclusive = 20000m, Percent = 7m },
            new ShopPlatformFeeTier { MaxExclusive = 30000m, Percent = 6m },
            new ShopPlatformFeeTier { MaxExclusive = 40000m, Percent = 5m },
            new ShopPlatformFeeTier { MaxExclusive = 50000m, Percent = 4m },
            new ShopPlatformFeeTier { MaxExclusive = 100000m, Percent = 3m },
            new ShopPlatformFeeTier { MaxExclusive = 500000m, Percent = 2m },
            new ShopPlatformFeeTier { MaxExclusive = null, Percent = 1m }
        };

        public static ShopPlatformFeeResult Calculate(decimal orderPrice, IReadOnlyList<ShopPlatformFeeTier>? tiers = null)
        {
            if (orderPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orderPrice), "Order price cannot be negative.");
            }

            var source = tiers == null || tiers.Count == 0 ? DefaultTiers() : tiers;
            var ordered = source
                .OrderBy(tier => tier.MaxExclusive.HasValue ? 0 : 1)
                .ThenBy(tier => tier.MaxExclusive)
                .ToList();

            ShopPlatformFeeTier? match = null;
            foreach (var tier in ordered)
            {
                if (tier.Percent < 0 || tier.Percent > 100)
                {
                    throw new InvalidOperationException($"Shop platform fee percent must be between 0 and 100. Got {tier.Percent}.");
                }

                if (!tier.MaxExclusive.HasValue || orderPrice < tier.MaxExclusive.Value)
                {
                    match = tier;
                    break;
                }
            }

            if (match == null)
            {
                throw new InvalidOperationException($"No shop platform fee tier covers order price {orderPrice}.");
            }

            var fee = decimal.Round(orderPrice * match.Percent / 100m, 2, MidpointRounding.AwayFromZero);
            return new ShopPlatformFeeResult(orderPrice, match.Percent, fee);
        }
    }
}

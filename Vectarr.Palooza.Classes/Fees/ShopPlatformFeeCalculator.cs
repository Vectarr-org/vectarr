namespace Vectarr.Palooza.Classes.Fees
{
    public class ShopPlatformFeeTier
    {
        /// <summary>
        /// Inclusive upper bound of the order price, in the same currency as the order.
        /// Null means no upper bound (the top tier).
        /// </summary>
        public decimal? UpTo { get; set; }

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
    /// Single calculator for the shop platform fee. A tier applies when the order price
    /// is less than or equal to that tier's upper bound (and above the previous bound).
    /// Exactly 1000 is 10%. Exactly 500000 is 2%. Above 500000 is 1%.
    /// There is no 1.5% band.
    ///
    /// Nothing in the API charges this yet. Call <see cref="Calculate"/> when a shop payout
    /// is taken, using the accepted order price (the accepted bid amount, or
    /// WorkOrder.CurrentPrice once that field is maintained). Do not use it for the
    /// instant-quote buyer deposit.
    /// </summary>
    public static class ShopPlatformFeeCalculator
    {
        public static List<ShopPlatformFeeTier> DefaultTiers() => new()
        {
            new ShopPlatformFeeTier { UpTo = 1000m, Percent = 10m },
            new ShopPlatformFeeTier { UpTo = 5000m, Percent = 9m },
            new ShopPlatformFeeTier { UpTo = 10000m, Percent = 8m },
            new ShopPlatformFeeTier { UpTo = 20000m, Percent = 7m },
            new ShopPlatformFeeTier { UpTo = 30000m, Percent = 6m },
            new ShopPlatformFeeTier { UpTo = 40000m, Percent = 5m },
            new ShopPlatformFeeTier { UpTo = 50000m, Percent = 4m },
            new ShopPlatformFeeTier { UpTo = 100000m, Percent = 3m },
            new ShopPlatformFeeTier { UpTo = 500000m, Percent = 2m },
            new ShopPlatformFeeTier { UpTo = null, Percent = 1m }
        };

        public static ShopPlatformFeeResult Calculate(decimal orderPrice, IReadOnlyList<ShopPlatformFeeTier>? tiers = null)
        {
            if (orderPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orderPrice), "Order price cannot be negative.");
            }

            var source = tiers == null || tiers.Count == 0 ? DefaultTiers() : tiers;
            var ordered = source
                .OrderBy(tier => tier.UpTo.HasValue ? 0 : 1)
                .ThenBy(tier => tier.UpTo)
                .ToList();

            ShopPlatformFeeTier? match = null;
            foreach (var tier in ordered)
            {
                if (tier.Percent < 0 || tier.Percent > 100)
                {
                    throw new InvalidOperationException($"Shop platform fee percent must be between 0 and 100. Got {tier.Percent}.");
                }

                if (!tier.UpTo.HasValue || orderPrice <= tier.UpTo.Value)
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

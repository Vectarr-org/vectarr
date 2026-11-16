using System.Globalization;
using System.Text.Json;
using Vectarr.Palooza.Classes.Fees;
using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class ShopPlatformFeeCalculatorTests
    {
        [Theory]
        [InlineData("0", "10")]
        [InlineData("1", "10")]
        [InlineData("999.99", "10")]
        [InlineData("1000", "9")]
        [InlineData("1000.01", "9")]
        [InlineData("4999.99", "9")]
        [InlineData("5000", "8")]
        [InlineData("5000.01", "8")]
        [InlineData("9999.99", "8")]
        [InlineData("10000", "7")]
        [InlineData("19999.99", "7")]
        [InlineData("20000", "6")]
        [InlineData("29999.99", "6")]
        [InlineData("30000", "5")]
        [InlineData("39999.99", "5")]
        [InlineData("40000", "4")]
        [InlineData("49999.99", "4")]
        [InlineData("50000", "3")]
        [InlineData("99999.99", "3")]
        [InlineData("100000", "2")]
        [InlineData("200000", "2")]
        [InlineData("499999.99", "2")]
        [InlineData("500000", "1")]
        [InlineData("500000.01", "1")]
        [InlineData("1000000", "1")]
        public void PercentFollowsHalfOpenBounds(string priceText, string percentText)
        {
            var price = decimal.Parse(priceText, CultureInfo.InvariantCulture);
            var percent = decimal.Parse(percentText, CultureInfo.InvariantCulture);
            var result = ShopPlatformFeeCalculator.Calculate(price);
            Assert.Equal(percent, result.Percent);
            Assert.Equal(decimal.Round(price * percent / 100m, 2, MidpointRounding.AwayFromZero), result.Fee);
        }

        [Fact]
        public void ThereIsNoOneAndAHalfPercentTier()
        {
            Assert.DoesNotContain(ShopPlatformFeeCalculator.DefaultTiers(), tier => tier.Percent == 1.5m);
            Assert.Equal(2m, ShopPlatformFeeCalculator.Calculate(250000m).Percent);
        }

        [Fact]
        public void RoundsTheFeeAwayFromZero()
        {
            var result = ShopPlatformFeeCalculator.Calculate(1.25m);
            Assert.Equal(10m, result.Percent);
            Assert.Equal(0.13m, result.Fee);
        }

        [Fact]
        public void NegativePriceIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ShopPlatformFeeCalculator.Calculate(-0.01m));
        }

        [Fact]
        public void AppsettingsTiersMatchTheCalculatorAndLeaveTheBuyerDepositAlone()
        {
            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Vectarr.Palooza.WebApi", "appsettings.json"));
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var root = document.RootElement;

            Assert.Equal(40m, root.GetProperty("InstantQuote").GetProperty("InitialFeePercent").GetDecimal());

            var configured = new List<ShopPlatformFeeTier>();
            foreach (var tier in root.GetProperty("ShopPlatformFee").GetProperty("Tiers").EnumerateArray())
            {
                decimal? maxExclusive = tier.GetProperty("MaxExclusive").ValueKind == JsonValueKind.Null
                    ? null
                    : tier.GetProperty("MaxExclusive").GetDecimal();
                configured.Add(new ShopPlatformFeeTier
                {
                    MaxExclusive = maxExclusive,
                    Percent = tier.GetProperty("Percent").GetDecimal()
                });
            }

            foreach (var price in new decimal[] { 0m, 999.99m, 1000m, 5000m, 50000m, 100000m, 200000m, 499999.99m, 500000m, 500000.01m })
            {
                Assert.Equal(
                    ShopPlatformFeeCalculator.Calculate(price).Percent,
                    ShopPlatformFeeCalculator.Calculate(price, configured).Percent);
            }
        }
    }
}

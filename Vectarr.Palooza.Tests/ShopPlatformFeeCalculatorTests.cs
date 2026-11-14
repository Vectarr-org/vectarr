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
        [InlineData("1000", "10")]
        [InlineData("1000.01", "9")]
        [InlineData("5000", "9")]
        [InlineData("5000.01", "8")]
        [InlineData("10000", "8")]
        [InlineData("10000.01", "7")]
        [InlineData("20000", "7")]
        [InlineData("20000.01", "6")]
        [InlineData("30000", "6")]
        [InlineData("30000.01", "5")]
        [InlineData("40000", "5")]
        [InlineData("40000.01", "4")]
        [InlineData("50000", "4")]
        [InlineData("50000.01", "3")]
        [InlineData("100000", "3")]
        [InlineData("100000.01", "2")]
        [InlineData("200000", "2")]
        [InlineData("499999.99", "2")]
        [InlineData("500000", "2")]
        [InlineData("500000.01", "1")]
        [InlineData("1000000", "1")]
        public void PercentFollowsInclusiveUpperBounds(string priceText, string percentText)
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
                decimal? upTo = tier.GetProperty("UpTo").ValueKind == JsonValueKind.Null
                    ? null
                    : tier.GetProperty("UpTo").GetDecimal();
                configured.Add(new ShopPlatformFeeTier
                {
                    UpTo = upTo,
                    Percent = tier.GetProperty("Percent").GetDecimal()
                });
            }

            foreach (var price in new decimal[] { 0m, 1000m, 1000.01m, 50000m, 100000m, 200000m, 500000m, 500000.01m })
            {
                Assert.Equal(
                    ShopPlatformFeeCalculator.Calculate(price).Percent,
                    ShopPlatformFeeCalculator.Calculate(price, configured).Percent);
            }
        }
    }
}

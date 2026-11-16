using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class BuyerEmailPrivacyTests
    {
        [Theory]
        [InlineData("QuoteRequestReceipt.html")]
        [InlineData("EmailVerification.html")]
        [InlineData("ForgotPassword.html")]
        [InlineData("Header.html")]
        [InlineData("Footer.html")]
        public void BuyerTemplatesDoNotCarryAShopPhoneOrStreet(string fileName)
        {
            var path = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "Vectarr.Palooza.ApiBusiness", "Emailing", "EmailTemplates", fileName));
            var html = File.ReadAllText(path);

            Assert.DoesNotContain("PhoneNumber", html, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Street1", html, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Street2", html, StringComparison.OrdinalIgnoreCase);
        }
    }
}

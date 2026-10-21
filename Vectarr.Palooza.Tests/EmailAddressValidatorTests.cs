using Vectarr.Palooza.CoreClasses;
using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class EmailAddressValidatorTests
    {
        [Theory]
        [InlineData("izzie@jinfengmould.cn")]
        [InlineData("shop@example.de")]
        [InlineData("shop@example.co.uk")]
        [InlineData("shop@example.io")]
        [InlineData("shop@example.com")]
        [InlineData("shop@example.net")]
        [InlineData("shop@example.org")]
        [InlineData("shop@example.gov")]
        [InlineData("Izzie@JinFengMould.CN")]
        [InlineData("user.name+bids@my-shop.io")]
        public void AcceptsAnyDomainAndTld(string email)
        {
            Assert.True(EmailAddressValidator.IsValid(email));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not-an-email")]
        [InlineData("izzie@jinfengmould")]
        [InlineData("izzie@jinfengmould.c")]
        [InlineData("izzie @jinfengmould.cn")]
        [InlineData("@jinfengmould.cn")]
        [InlineData("izzie@.cn")]
        [InlineData("izzie@-shop.cn")]
        [InlineData("izzie@shop-.cn")]
        public void RejectsMissingOrMalformedAddresses(string? email)
        {
            Assert.False(EmailAddressValidator.IsValid(email));
        }
    }
}

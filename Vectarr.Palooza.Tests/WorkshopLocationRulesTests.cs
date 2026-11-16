using System.ComponentModel.DataAnnotations;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Privacy;
using Vectarr.Palooza.CoreClasses.Enums;
using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class WorkshopLocationRulesTests
    {
        [Fact]
        public void PhoneCityStateAndZipAreRequiredAndStreetIsOptional()
        {
            var errors = WorkshopLocationRules.Validate(new AddressModel
            {
                City = "Austin",
                State = "TX",
                PostalCode = "78701"
            }, phoneNumber: "512-555-0142");

            Assert.Empty(errors);
        }

        [Fact]
        public void MissingPhoneIsRejected()
        {
            var errors = WorkshopLocationRules.Validate(new AddressModel
            {
                City = "Austin",
                State = "TX",
                PostalCode = "78701",
                Street1 = "10 Shop St"
            }, phoneNumber: "  ");

            Assert.Contains("Phone number is required.", errors);
        }

        [Fact]
        public void MissingCityStateOrZipIsRejectedWithoutRequiringStreet()
        {
            var errors = WorkshopLocationRules.Validate(new AddressModel
            {
                Street1 = null,
                City = "",
                State = "TX"
            }, phoneNumber: "512-555-0142");

            Assert.Contains("City is required.", errors);
            Assert.Contains("ZIP code is required.", errors);
            Assert.DoesNotContain(errors, error => error.Contains("Street", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void MissingAddressReportsCityStateAndZip()
        {
            var errors = WorkshopLocationRules.Validate(null, "512-555-0142");
            Assert.Contains("City, state, and ZIP code are required.", errors);
        }

        [Fact]
        public void ValidationDoesNotClearAProvidedStreet()
        {
            var address = new AddressModel
            {
                City = "Austin",
                State = "TX",
                PostalCode = "78701",
                Street1 = "10 Shop St"
            };

            WorkshopLocationRules.Validate(address, "512-555-0142");
            Assert.Equal("10 Shop St", address.Street1);
        }

        [Fact]
        public void PartnerLeadFormStillRequiresAPhone()
        {
            var phone = typeof(BecomeAPartnerRequest).GetProperty(nameof(BecomeAPartnerRequest.PhoneNumber));
            Assert.NotNull(phone);
            Assert.Contains(phone!.GetCustomAttributes(inherit: true), attribute => attribute is RequiredAttribute);
        }
    }
}

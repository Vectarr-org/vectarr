using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Privacy;
using Vectarr.Palooza.CoreClasses.Enums;
using Xunit;

namespace Vectarr.Palooza.Tests
{
    public class MarketplacePrivacyTests
    {
        [Fact]
        public void BuyerSeesShopCityAndStateOnly()
        {
            var shop = SampleShop();
            MarketplacePrivacy.Apply(shop, MarketplaceViewer.Buyer());

            Assert.Equal("Austin", shop.Address!.City);
            Assert.Equal("TX", shop.Address.State);
            Assert.Equal(CountryEnum.UnitedStates, shop.Address.Country);
            Assert.Null(shop.Address.Street1);
            Assert.Null(shop.Address.Street2);
            Assert.Null(shop.Address.PostalCode);
            Assert.Null(shop.PhoneNumber);
            Assert.Null(shop.Email);
            Assert.Equal("North Shop", shop.Name);
        }

        [Fact]
        public void OwningShopAndAdminStillSeePhoneStreetAndEmail()
        {
            var ownerView = SampleShop();
            MarketplacePrivacy.Apply(ownerView, MarketplaceViewer.Shop(ownerView.Id));
            AssertFullShop(ownerView);

            var adminView = SampleShop();
            MarketplacePrivacy.Apply(adminView, MarketplaceViewer.Admin());
            AssertFullShop(adminView);
        }

        [Fact]
        public void AnotherShopDoesNotSeePhoneOrStreet()
        {
            var shop = SampleShop();
            MarketplacePrivacy.Apply(shop, MarketplaceViewer.Shop(999));

            Assert.Null(shop.PhoneNumber);
            Assert.Null(shop.Email);
            Assert.Null(shop.Address!.Street1);
            Assert.Equal("Austin", shop.Address.City);
        }

        [Fact]
        public void BuyerJobAndBidHideShopPhoneAndKeepTheBuyersOwnAddress()
        {
            var job = new JobModel
            {
                Id = 5,
                Creator = new CreatorModel { Address = BuyerAddress() },
                Bids = new List<BidModel>
                {
                    new BidModel
                    {
                        Id = 1,
                        Workshop = SampleShop(),
                        Job = new JobModel { Id = 5 }
                    }
                }
            };

            MarketplacePrivacy.Apply(job, MarketplaceViewer.Buyer(identityId: 42));

            Assert.Equal("500 Buyer Ave", job.Creator!.Address!.Street1);
            Assert.Equal("78702", job.Creator.Address.PostalCode);
            Assert.Null(job.Bids![0].Workshop!.PhoneNumber);
            Assert.Null(job.Bids[0].Workshop.Email);
            Assert.Null(job.Bids[0].Workshop!.Address!.Street1);
            Assert.Equal("Austin", job.Bids[0].Workshop.Address!.City);
        }

        [Fact]
        public void BrowsingAndBiddingShopsDoNotSeeTheExactShipTo()
        {
            var browsing = JobWithBuyerAddress(awardedWorkshopId: null);
            MarketplacePrivacy.Apply(browsing, MarketplaceViewer.Shop(7));
            AssertCityStateOnly(browsing.Creator!.Address!);

            var bidding = JobWithBuyerAddress(awardedWorkshopId: null);
            var bid = new BidModel { Workshop = SampleShop(), Job = bidding };
            MarketplacePrivacy.Apply(bid, MarketplaceViewer.Shop(7));
            AssertCityStateOnly(bid.Job.Creator!.Address!);

            var lost = JobWithBuyerAddress(awardedWorkshopId: 8);
            MarketplacePrivacy.Apply(lost, MarketplaceViewer.Shop(7));
            AssertCityStateOnly(lost.Creator!.Address!);
        }

        [Fact]
        public void WinningShopSeesTheExactShipToOnTheAcceptedOrder()
        {
            var order = new WorkOrderModel
            {
                Id = 3,
                WorkshopId = 7,
                Status = WorkOrderStatusEnum.CreatorAccepted,
                ShippingAddress = BuyerAddress(),
                AcceptedBid = new BidModel { Workshop = SampleShop() },
                Job = new JobModel
                {
                    Creator = new CreatorModel { Address = BuyerAddress() }
                }
            };

            MarketplacePrivacy.Apply(order, MarketplaceViewer.Shop(7));

            Assert.Equal("500 Buyer Ave", order.ShippingAddress!.Street1);
            Assert.Equal("78702", order.ShippingAddress.PostalCode);
            Assert.Equal("500 Buyer Ave", order.Job!.Creator!.Address!.Street1);
            Assert.Equal("512-555-0142", order.AcceptedBid.Workshop.PhoneNumber);

            var buyerView = new WorkOrderModel
            {
                WorkshopId = 7,
                Status = WorkOrderStatusEnum.CreatorAccepted,
                AcceptedBid = new BidModel { Workshop = SampleShop() }
            };
            MarketplacePrivacy.Apply(buyerView, MarketplaceViewer.Buyer());
            Assert.Null(buyerView.AcceptedBid.Workshop.PhoneNumber);
        }

        [Theory]
        [InlineData(WorkOrderStatusEnum.CreatorAccepted)]
        [InlineData(WorkOrderStatusEnum.Active)]
        [InlineData(WorkOrderStatusEnum.Completed)]
        public void AcceptedStatusesRevealTheShipToToTheWinner(WorkOrderStatusEnum status)
        {
            var order = new WorkOrderModel
            {
                WorkshopId = 7,
                Status = status,
                ShippingAddress = BuyerAddress()
            };

            MarketplacePrivacy.Apply(order, MarketplaceViewer.Shop(7));
            Assert.Equal("500 Buyer Ave", order.ShippingAddress!.Street1);
        }

        [Fact]
        public void NonAwardedShopDoesNotReceiveAnotherOrdersShipTo()
        {
            var order = new WorkOrderModel
            {
                WorkshopId = 8,
                Status = WorkOrderStatusEnum.Active,
                ShippingAddress = BuyerAddress()
            };

            MarketplacePrivacy.Apply(order, MarketplaceViewer.Shop(7));
            Assert.Null(order.ShippingAddress);
        }

        [Fact]
        public void RedactionDoesNotChangeTheStoredCheckoutAddress()
        {
            var stored = BuyerAddress();
            var response = new AddressModel
            {
                Street1 = stored.Street1,
                Street2 = stored.Street2,
                City = stored.City,
                State = stored.State,
                PostalCode = stored.PostalCode
            };

            MarketplacePrivacy.RedactExactShipping(response, canSeeExact: false);

            Assert.Equal("500 Buyer Ave", stored.Street1);
            Assert.Equal("78702", stored.PostalCode);
            Assert.Null(response.Street1);
            Assert.Null(response.PostalCode);
            Assert.Equal("Austin", response.City);
        }

        [Fact]
        public void BuyerDoesNotSeeAnotherPersonsPhoneOrEmail()
        {
            var identity = new IdentityModel
            {
                Id = 9,
                EmailAddress = "owner@shop.example",
                PhoneNumber = "512-555-0199",
                FirstName = "Ada",
                Address = new AddressModel
                {
                    Street1 = "9 Shop St",
                    City = "Austin",
                    State = "TX",
                    PostalCode = "78701"
                }
            };

            MarketplacePrivacy.Apply(identity, MarketplaceViewer.Buyer(identityId: 3));

            Assert.Null(identity.PhoneNumber);
            Assert.Null(identity.EmailAddress);
            Assert.Null(identity.Address!.Street1);
            Assert.Null(identity.Address.PostalCode);
            Assert.Equal("Austin", identity.Address.City);
            Assert.Equal("Ada", identity.FirstName);
        }

        [Fact]
        public void AnotherShopDoesNotSeeAShopPhoneOnAnIdentity()
        {
            var identity = new IdentityModel
            {
                Id = 9,
                EmailAddress = "owner@shop.example",
                PhoneNumber = "512-555-0199",
                Address = new AddressModel { City = "Austin", State = "TX", Street1 = "9 Shop St", PostalCode = "78701" }
            };

            MarketplacePrivacy.Apply(identity, MarketplaceViewer.Shop(4, identityId: 2));

            Assert.Null(identity.PhoneNumber);
            Assert.Null(identity.EmailAddress);
            Assert.Null(identity.Address!.Street1);
            Assert.Equal("Austin", identity.Address.City);
        }

        [Fact]
        public void BuyerStillSeesTheirOwnCheckoutIdentity()
        {
            var identity = new IdentityModel
            {
                Id = 3,
                EmailAddress = "buyer@example.com",
                PhoneNumber = "512-555-0100",
                Address = BuyerAddress()
            };

            MarketplacePrivacy.Apply(identity, MarketplaceViewer.Buyer(identityId: 3));

            Assert.Equal("buyer@example.com", identity.EmailAddress);
            Assert.Equal("512-555-0100", identity.PhoneNumber);
            Assert.Equal("500 Buyer Ave", identity.Address!.Street1);
        }

        [Fact]
        public void BuyerNoticeOmitsPhoneEmailStreetAndZip()
        {
            var shop = SampleShop();
            var notice = MarketplacePrivacy.BuyerNotice(shop);

            Assert.Contains("North Shop", notice);
            Assert.Contains("Austin", notice);
            Assert.Contains("TX", notice);
            Assert.DoesNotContain(shop.PhoneNumber!, notice);
            Assert.DoesNotContain(shop.Email!, notice);
            Assert.DoesNotContain(shop.Address!.Street1!, notice);
            Assert.DoesNotContain(shop.Address.PostalCode!, notice);
        }

        [Fact]
        public void UnknownViewerSeesNeitherShopPhoneNorExactShipTo()
        {
            var shop = SampleShop();
            MarketplacePrivacy.Apply(shop, MarketplaceViewer.Unknown());
            Assert.Null(shop.PhoneNumber);
            Assert.Null(shop.Address!.Street1);

            var job = JobWithBuyerAddress(awardedWorkshopId: 7);
            MarketplacePrivacy.Apply(job, MarketplaceViewer.Unknown());
            AssertCityStateOnly(job.Creator!.Address!);
        }

        private static void AssertFullShop(WorkshopModel shop)
        {
            Assert.Equal("512-555-0142", shop.PhoneNumber);
            Assert.Equal("ada@northshop.example", shop.Email);
            Assert.Equal("10 Shop St", shop.Address!.Street1);
            Assert.Equal("78701", shop.Address.PostalCode);
            Assert.Equal("Austin", shop.Address.City);
        }

        private static void AssertCityStateOnly(AddressModel address)
        {
            Assert.Equal("Austin", address.City);
            Assert.Equal("TX", address.State);
            Assert.Null(address.Street1);
            Assert.Null(address.Street2);
            Assert.Null(address.PostalCode);
        }

        private static JobModel JobWithBuyerAddress(int? awardedWorkshopId)
        {
            return new JobModel
            {
                AwardedWorkshopId = awardedWorkshopId,
                Creator = new CreatorModel { Address = BuyerAddress() }
            };
        }

        private static WorkshopModel SampleShop()
        {
            return new WorkshopModel
            {
                Id = 7,
                Name = "North Shop",
                Email = "ada@northshop.example",
                PhoneNumber = "512-555-0142",
                Address = new AddressModel
                {
                    Country = CountryEnum.UnitedStates,
                    Street1 = "10 Shop St",
                    Street2 = "Suite 2",
                    City = "Austin",
                    State = "TX",
                    PostalCode = "78701"
                }
            };
        }

        private static AddressModel BuyerAddress()
        {
            return new AddressModel
            {
                Street1 = "500 Buyer Ave",
                Street2 = "Apt 4",
                City = "Austin",
                State = "TX",
                PostalCode = "78702",
                Country = CountryEnum.UnitedStates
            };
        }
    }
}

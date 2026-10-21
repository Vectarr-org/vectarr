using Vectarr.Palooza.CoreClasses;
using Xunit;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Tests
{
    public class SourcingRegionPolicyTests
    {
        [Fact]
        public void ItarOrExportControlled_ForcesUsOnly_EvenWhenClientRequestsAnyRegion()
        {
            Assert.Equal(
                SourcingRegionEnum.UsShopsOnly,
                SourcingRegionPolicy.Resolve(SourcingRegionEnum.AnyRegion, isItar: true, isExportControlled: false));
            Assert.Equal(
                SourcingRegionEnum.UsShopsOnly,
                SourcingRegionPolicy.Resolve(SourcingRegionEnum.AnyRegion, isItar: false, isExportControlled: true));
            Assert.Equal(
                SourcingRegionEnum.UsShopsOnly,
                SourcingRegionPolicy.Resolve(SourcingRegionEnum.UsShopsOnly, isItar: true, isExportControlled: true));
        }

        [Fact]
        public void UncontrolledJob_KeepsTheBuyerSourcingChoice()
        {
            Assert.Equal(
                SourcingRegionEnum.AnyRegion,
                SourcingRegionPolicy.Resolve(SourcingRegionEnum.AnyRegion, isItar: false, isExportControlled: false));
            Assert.Equal(
                SourcingRegionEnum.UsShopsOnly,
                SourcingRegionPolicy.Resolve(SourcingRegionEnum.UsShopsOnly, isItar: false, isExportControlled: false));
        }

        [Fact]
        public void NonUsShops_CannotSeeOrBid_OnUsOnlyOrControlledJobs()
        {
            Assert.False(SourcingRegionPolicy.CanShopAccess(CountryEnum.China, SourcingRegionEnum.UsShopsOnly, false, false));
            Assert.False(SourcingRegionPolicy.CanShopAccess(CountryEnum.China, SourcingRegionEnum.AnyRegion, true, false));
            Assert.False(SourcingRegionPolicy.CanShopAccess(CountryEnum.China, SourcingRegionEnum.AnyRegion, false, true));
            Assert.False(SourcingRegionPolicy.CanShopAccess(null, SourcingRegionEnum.UsShopsOnly, false, false));
            Assert.False(SourcingRegionPolicy.CanShopAccess(null, SourcingRegionEnum.AnyRegion, true, false));

            Assert.True(SourcingRegionPolicy.CanShopAccess(CountryEnum.UnitedStates, SourcingRegionEnum.UsShopsOnly, true, true));
            Assert.True(SourcingRegionPolicy.CanShopAccess(CountryEnum.China, SourcingRegionEnum.AnyRegion, false, false));
            Assert.True(SourcingRegionPolicy.CanShopAccess(CountryEnum.Germany, SourcingRegionEnum.AnyRegion, false, false));
            Assert.True(SourcingRegionPolicy.CanShopAccess(null, SourcingRegionEnum.AnyRegion, false, false));
        }

        [Fact]
        public void ListingFilter_HidesUsOnlyAndControlledJobs_FromNonUsShops()
        {
            var jobs = new[]
            {
                new SampleJob(1, SourcingRegionEnum.AnyRegion, false, false),
                new SampleJob(2, SourcingRegionEnum.UsShopsOnly, false, false),
                new SampleJob(3, SourcingRegionEnum.AnyRegion, true, false),
                new SampleJob(4, SourcingRegionEnum.AnyRegion, false, true),
            };

            var visibleToChina = SourcingRegionPolicy.FilterVisible(
                jobs,
                CountryEnum.China,
                job => job.Region,
                job => job.IsItar,
                job => job.IsExportControlled).Select(job => job.Id).ToList();
            var visibleToUs = SourcingRegionPolicy.FilterVisible(
                jobs,
                CountryEnum.UnitedStates,
                job => job.Region,
                job => job.IsItar,
                job => job.IsExportControlled).Select(job => job.Id).ToList();

            Assert.Equal(new[] { 1 }, visibleToChina);
            Assert.Equal(new[] { 1, 2, 3, 4 }, visibleToUs);
        }

        [Fact]
        public void ShopJobQueryPredicate_MatchesAccessPolicy_ForEveryFlagCombination()
        {
            var countries = new CountryEnum?[] { null, CountryEnum.UnitedStates, CountryEnum.China, CountryEnum.Germany };
            var regions = new[] { SourcingRegionEnum.AnyRegion, SourcingRegionEnum.UsShopsOnly };

            foreach (var country in countries)
            {
                foreach (var region in regions)
                {
                    foreach (var isItar in new[] { false, true })
                    {
                        foreach (var isExportControlled in new[] { false, true })
                        {
                            Assert.Equal(
                                SourcingRegionPolicy.CanShopAccess(country, region, isItar, isExportControlled),
                                SourcingRegionPolicy.VisibleInShopJobQuery(country, region, isItar, isExportControlled));
                        }
                    }
                }
            }
        }

        [Fact]
        public void CountryCode_ResolvesIsoAlpha2_AndPrefersItOverTheEnum()
        {
            Assert.Equal(CountryEnum.China, SourcingRegionPolicy.ResolveCountry(null, "cn"));
            Assert.Equal(CountryEnum.UnitedStates, SourcingRegionPolicy.ResolveCountry(null, "US"));
            Assert.Equal(CountryEnum.Germany, SourcingRegionPolicy.ResolveCountry(CountryEnum.Germany, null));
            Assert.Equal(CountryEnum.China, SourcingRegionPolicy.ResolveCountry(CountryEnum.UnitedStates, "CN"));
            Assert.Null(SourcingRegionPolicy.ResolveCountry(null, "ZZ"));
        }

        [Fact]
        public void LandedCost_AddsPartPriceShippingAndDutyEstimate()
        {
            Assert.Equal(130m, BidPricing.LandedCost(100m, 20m, 10m));
            Assert.Equal(120m, BidPricing.LandedCost(100m, 20m, 0m));
            Assert.Equal(0m, BidPricing.ResolveDutyEstimate(0m, null));
            Assert.Equal(8m, BidPricing.ResolveDutyEstimate(0m, 8m));
            Assert.Equal(4m, BidPricing.ResolveDutyEstimate(4m, 8m));
        }

        private sealed record SampleJob(int Id, SourcingRegionEnum Region, bool IsItar, bool IsExportControlled);
    }
}

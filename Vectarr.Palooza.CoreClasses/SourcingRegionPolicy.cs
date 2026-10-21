using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.CoreClasses
{
    /// <summary>
    /// Server-side rules for US-only sourcing. ITAR and export-controlled RFQs are always US-only,
    /// and a shop whose country is not the United States cannot see or bid on those RFQs.
    /// A missing shop country is treated as non-US.
    /// </summary>
    public static class SourcingRegionPolicy
    {
        public static bool IsUnitedStates(CountryEnum? country)
        {
            return country == CountryEnum.UnitedStates;
        }

        /// <summary>
        /// Buyer choice, overwritten when the job is ITAR or export-controlled.
        /// The client cannot keep <see cref="SourcingRegionEnum.AnyRegion"/> on those jobs.
        /// </summary>
        public static SourcingRegionEnum Resolve(SourcingRegionEnum requested, bool isItar, bool isExportControlled)
        {
            if (isItar || isExportControlled)
            {
                return SourcingRegionEnum.UsShopsOnly;
            }

            return requested;
        }

        public static bool CanShopAccess(CountryEnum? shopCountry, SourcingRegionEnum requested, bool isItar, bool isExportControlled)
        {
            var effective = Resolve(requested, isItar, isExportControlled);
            if (effective != SourcingRegionEnum.UsShopsOnly)
            {
                return true;
            }

            return IsUnitedStates(shopCountry);
        }

        /// <summary>
        /// Boolean used by the available-jobs query. Kept equal to <see cref="CanShopAccess"/>
        /// so the SQL filter and the in-memory filter cannot drift.
        /// </summary>
        public static bool VisibleInShopJobQuery(CountryEnum? shopCountry, SourcingRegionEnum sourcingRegion, bool isItar, bool isExportControlled)
        {
            return shopCountry == CountryEnum.UnitedStates
                || (sourcingRegion != SourcingRegionEnum.UsShopsOnly && !isItar && !isExportControlled);
        }

        public static IEnumerable<T> FilterVisible<T>(
            IEnumerable<T> jobs,
            CountryEnum? shopCountry,
            Func<T, SourcingRegionEnum> sourcingRegion,
            Func<T, bool> isItar,
            Func<T, bool> isExportControlled)
        {
            return jobs.Where(job => CanShopAccess(shopCountry, sourcingRegion(job), isItar(job), isExportControlled(job)));
        }

        /// <summary>
        /// Prefers a valid ISO 3166-1 alpha-2 code, then the country enum.
        /// </summary>
        public static CountryEnum? ResolveCountry(CountryEnum? country, string? countryCode)
        {
            var fromCode = new CountryWrapper().TryFromAlpha2(countryCode);
            if (fromCode.HasValue)
            {
                return fromCode;
            }

            if (country.HasValue && Enum.IsDefined(country.Value))
            {
                return country.Value;
            }

            return null;
        }
    }
}

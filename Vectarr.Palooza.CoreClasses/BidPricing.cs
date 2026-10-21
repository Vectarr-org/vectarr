namespace Vectarr.Palooza.CoreClasses
{
    /// <summary>
    /// Landed cost is part price + shipping + the duties/tariff estimate.
    /// Duty figures are estimates for comparison, not amounts Vectarr collects.
    /// </summary>
    public static class BidPricing
    {
        public static decimal ResolveDutyEstimate(decimal dutyEstimate, decimal? legacyTarrifs)
        {
            if (dutyEstimate != 0m)
            {
                return dutyEstimate;
            }

            return legacyTarrifs ?? 0m;
        }

        public static decimal LandedCost(decimal partPrice, decimal shipping, decimal dutyEstimate)
        {
            return partPrice + shipping + dutyEstimate;
        }
    }
}

namespace Vectarr.Palooza.CoreClasses.Enums
{
    /// <summary>
    /// Buyer choice for which shops may see and bid on an RFQ/Job.
    /// ITAR and export-controlled jobs are forced to <see cref="UsShopsOnly"/> on the server.
    /// </summary>
    public enum SourcingRegionEnum
    {
        /// <summary>Shops in any country may see and bid.</summary>
        AnyRegion = 0,

        /// <summary>Only shops whose country is the United States may see and bid.</summary>
        UsShopsOnly = 1
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    /// <summary>
    /// A request submitted from the in-house Instant Quote tool (CAD file + browser-computed price),
    /// paid through Stripe (Payment Element on the checkout page) and reviewed by a Vectarr admin.
    /// </summary>
    [Table("Quote.QuoteRequests")]
    public class QuoteRequest : BaseModel
    {
        /// <summary>Human friendly reference, e.g. VQ-261013-7KQ2M. Generated server side, unique.</summary>
        public string ReferenceNumber { get; set; } = null!;
        public QuoteRequestStatusEnum Status { get; set; }

        /// <summary>Secret (Guid "N") that lets a guest load their own request. Generated server side.</summary>
        public string AccessToken { get; set; } = null!;

        public int? IdentityId { get; set; }
        public virtual Identity? Identity { get; set; }

        /// <summary>Reserved for linking the request to a workshop job later.</summary>
        public int? JobId { get; set; }
        public virtual Job? Job { get; set; }

        // Contact
        public string ContactName { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
        public string? Company { get; set; }
        public string? Phone { get; set; }

        // Shipping
        public string? ShippingStreet1 { get; set; }
        public string? ShippingStreet2 { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }
        public string? ShippingPostalCode { get; set; }
        public string? ShippingCountry { get; set; }

        // Part
        /// <summary>Part name; defaults to the original CAD file name.</summary>
        public string? PartName { get; set; }
        public string? PartNumber { get; set; }
        public string? Revision { get; set; }
        public string? BuyerNotes { get; set; }
        public string? SourceFormat { get; set; }
        public bool IsAssembly { get; set; }
        public int? AssemblyPartCount { get; set; }

        // Options selected in the instant quote tool
        public string? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? FinishId { get; set; }
        public string? FinishName { get; set; }
        public string? ToleranceId { get; set; }
        public string? ToleranceLabel { get; set; }
        public string? SpeedId { get; set; }
        public string? SpeedLabel { get; set; }
        /// <summary>Comma separated inspection document ids, e.g. "standard,fai".</summary>
        public string? InspectionDocs { get; set; }

        public int Quantity { get; set; }

        // Pricing
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        /// <summary>Initial payment computed server side from TotalPrice and InstantQuote config.</summary>
        public decimal AmountDue { get; set; }
        public decimal? AmountPaid { get; set; }
        public string Currency { get; set; } = "usd";

        public int? LeadDays { get; set; }
        public DateTime? ShipByDate { get; set; }

        // Geometry
        public double? BboxXmm { get; set; }
        public double? BboxYmm { get; set; }
        public double? BboxZmm { get; set; }
        public double? VolumeMm3 { get; set; }

        /// <summary>Full browser quote output (JSON text, max 2 MB).</summary>
        public string? QuoteSnapshotJson { get; set; }

        // Files (stored through the existing Image entity)
        public int? CadFileId { get; set; }
        public virtual Image? CadFile { get; set; }
        public string? CadFileName { get; set; }
        public int? DrawingFileId { get; set; }
        public virtual Image? DrawingFile { get; set; }
        public string? DrawingFileName { get; set; }

        // Payment
        public string? StripeCheckoutSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public DateTime? PaidAt { get; set; }

        /// <summary>Internal notes, only visible to Vectarr admins.</summary>
        public string? AdminNotes { get; set; }
    }
}

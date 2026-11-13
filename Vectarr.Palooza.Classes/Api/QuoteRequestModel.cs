using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// API shape of an instant quote request (serialized camelCase).
    /// AccessToken is only filled for Create and for the owner; AdminNotes only for Vectarr admins.
    /// QuoteSnapshotJson is only filled on single-item responses (not in lists).
    /// </summary>
    public class QuoteRequestModel : BaseApiModel
    {
        public string ReferenceNumber { get; set; } = null!;
        public QuoteRequestStatusEnum Status { get; set; }
        public string StatusText => Status.ToString();
        public string? AccessToken { get; set; }
        public int? IdentityId { get; set; }
        public int? JobId { get; set; }

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
        public string? PartName { get; set; }
        public string? PartNumber { get; set; }
        public string? Revision { get; set; }
        public string? BuyerNotes { get; set; }
        public string? SourceFormat { get; set; }
        public bool IsAssembly { get; set; }
        public int? AssemblyPartCount { get; set; }

        // Options
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
        public string? InspectionDocs { get; set; }

        public int Quantity { get; set; }

        // Pricing
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
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

        public string? QuoteSnapshotJson { get; set; }

        // Files
        public int? CadFileId { get; set; }
        public QuoteRequestFileModel? CadFile { get; set; }
        public int? DrawingFileId { get; set; }
        public QuoteRequestFileModel? DrawingFile { get; set; }

        // Payment
        public string? StripeCheckoutSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public DateTime? PaidAt { get; set; }

        public string? AdminNotes { get; set; }
    }

    /// <summary>
    /// An uploaded quote request file: the regular ImageModel fields plus the original file name.
    /// Use api/QuoteRequest/DownloadFile to download it.
    /// </summary>
    public class QuoteRequestFileModel : ImageModel
    {
        public string? FileName { get; set; }
    }
}

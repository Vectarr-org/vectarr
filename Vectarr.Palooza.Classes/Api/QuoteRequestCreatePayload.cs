using System.Text.Json;

namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// The fields a client may set when creating a quote request (the "payload" form field, JSON).
    /// Everything else (reference, token, status, amount due, currency, files, payment info) is server controlled.
    /// </summary>
    public class QuoteRequestCreatePayload
    {
        // Contact
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
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

        /// <summary>Comma separated string ("standard,fai") or an array of ids.</summary>
        public JsonElement? InspectionDocs { get; set; }

        public int Quantity { get; set; }

        // Prices (computed in the browser, range-validated on the server)
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public int? LeadDays { get; set; }
        public DateTime? ShipByDate { get; set; }

        // Geometry
        public double? BboxXmm { get; set; }
        public double? BboxYmm { get; set; }
        public double? BboxZmm { get; set; }
        public double? VolumeMm3 { get; set; }

        /// <summary>Full browser quote output: a JSON string, or a JSON object/array (stored as raw JSON text).</summary>
        public JsonElement? QuoteSnapshotJson { get; set; }
    }
}

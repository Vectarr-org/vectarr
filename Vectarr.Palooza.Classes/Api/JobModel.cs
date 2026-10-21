using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Api
{
    public class JobModel : BaseApiModel
    {
        public ToleranceEnum? Tolerance { get; set; }
        public string? ToleranceText => Tolerance.HasValue  ? new ToleranceEnumWrapper().GetDisplayText(Tolerance.Value) : null;
        public SourceFinishEnum? SourceFinish { get; set; }
        public string? SourceFinishText => SourceFinish.HasValue ? new SourceFinishEnumWrapper().GetDisplayText(SourceFinish.Value) : null;
        public string? AdditionalNotes { get; set; }
        public ManufacturingTypeEnum? ManufacturingType { get; set; }
        public string? ManufacturingTypeText => ManufacturingType.HasValue ? new ManufacturingTypeEnumWrapper().GetDisplayText(ManufacturingType.Value) : null;
        public MaterialEnum? Material { get; set; }
        public string? MaterialText => Material.HasValue ? new MaterialEnumWrapper().GetDisplayText(Material.Value) : null;
        public List<ImageModel> Files { get; set; }
        public List<JobQualityCheckRequestModel> QualityChecks { get; set; }
        public CostVsSpeedEnum CostVsSpeed { get; set; }
        public string CostVsSpeedText => new CostVsSpeedEnumWrapper().GetDisplayText(CostVsSpeed);
        public string CostVsSpeedDescription => new CostVsSpeedEnumWrapper().GetDisplayDescription(CostVsSpeed);
        public int Quantity { get; set; }
        public JobStatusEnum Status { get; set; }
        public string StatusText => new JobStatusEnumWrapper().GetDisplayText(Status);
        public DateTime? PublishDate { get; set; }
        public List<BidModel>? Bids { get; set; }
        public decimal? InitialPrice { get; set; }
        public bool ManualPricing { get; set; }
        public CreatorModel? Creator { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? WorkOrderId { get; set; }
        public int? QCPercentage { get; set; }
        public bool RequireQcConfirmation { get; set; }
        public bool AutoAcceptQcReport { get; set; }

        /// <summary>
        /// Buyer choice: 0 AnyRegion ("any region"), 1 UsShopsOnly ("US shops only").
        /// When <see cref="IsItar"/> or <see cref="IsExportControlled"/> is true the server stores UsShopsOnly
        /// and ignores a client value of AnyRegion.
        /// </summary>
        public SourcingRegionEnum SourcingRegion { get; set; }
        public string SourcingRegionText => new SourcingRegionEnumWrapper().GetDisplayText(SourcingRegion);
        public bool IsItar { get; set; }
        public bool IsExportControlled { get; set; }
    }
}

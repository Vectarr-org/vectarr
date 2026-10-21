using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.Jobs")]
    public class Job : BaseModel
    {
        public ManufacturingTypeEnum? ManufacturingType { get; set; }
        public MaterialEnum? Material { get; set; }
        public virtual ICollection<Image> Files { get; set; }
        public CostVsSpeedEnum CostVsSpeed { get; set; }
        public ToleranceEnum? Tolerance { get; set; }
        public SourceFinishEnum? SourceFinish { get; set; }
        public string? AdditionalNotes { get; set; }
        public int Quantity { get; set; }
        public int CreatorId { get; set; }
        public virtual Creator Creator { get; set; }
        public JobStatusEnum Status { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public int? WinningBidId { get; set; }
        public virtual Bid? WinningBid { get; set; }
        public DateTime? PublishDate { get; set; }
        public decimal? InitialPrice { get; set; }
        public bool ManualPricing { get; set; }
        public virtual WorkOrder? WorkOrder { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<JobQualityCheckRequest> QualityChecks { get; set; }
        public int? QCPrecentage { get; set; }
        public bool RequireQcConfirmation { get; set; }
        public bool AutoAcceptQcReport { get; set; }

        /// <summary>
        /// Buyer sourcing choice. Forced to US-only when <see cref="IsItar"/> or <see cref="IsExportControlled"/> is set.
        /// </summary>
        public SourcingRegionEnum SourcingRegion { get; set; }
        public bool IsItar { get; set; }
        public bool IsExportControlled { get; set; }
    }
}

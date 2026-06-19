using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkOrderQualityCheckRequestModel : BaseApiModel
    {
        public int WorkOrderId { get; set; }

        public QualityCheckTypeEnum CheckType { get; set; }
        public string CheckTypeText => new QualityCheckTypeEnumWrapper().GetDisplayText(CheckType);
        public string? Instructions { get; set; }
        public bool? Passed { get; set; }
        public string? ResultNotes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<ImageModel> Attachments { get; set; }
        public int RejectedCount { get; set; }
    }
}

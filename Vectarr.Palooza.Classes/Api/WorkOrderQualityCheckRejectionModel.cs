using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkOrderQualityCheckRejectionModel : BaseApiModel
    {
        public string? RejectionReason { get; set; }
        public int QualityCheckRequestId { get; set; }
    }
}

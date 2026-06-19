using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkOrderProgressUpdateModel : BaseApiModel
    {
        public int ProgressStageId { get; set; }
        public int PercentComplete { get; set; }
        public string? Notes { get; set; }
        public string? WorkshopCustomProgressMessageText { get; set; }
public int? WorkshopCustomProgressMessageId { get; set; }
        public List<ImageModel>? Images { get; set; }
    }
}

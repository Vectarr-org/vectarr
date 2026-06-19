using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.Api
{
    public class QualityCheckStatusModel
    {
        public DateTime StageStartedAt { get; set; }
        public DateTime CreatorDeadline { get; set; }
        public bool CreatorCanStillRequestChecks { get; set; }
        public bool HasRequests { get; set; }
        public int PendingRequestCount { get; set; }
        public bool AllRequestsCompleted { get; set; }
        public bool CanProceedToShipping { get; set; }
    }
}

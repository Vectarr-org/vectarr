using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.CoreClasses.Enums
{
    public enum ProgressStageEnum
    {
        JobStarted = 1,
        InProgress = 2,
        QualityCheck = 3,
        Shipping = 4,
        Received = 5,
        Completed = 6
    }
}

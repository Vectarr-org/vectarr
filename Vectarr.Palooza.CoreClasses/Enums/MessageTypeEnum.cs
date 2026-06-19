using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.CoreClasses.Enums
{
    public enum MessageTypeEnum
    {
        UserMessage,        // Regular chat message
        SystemEvent,        // "Job started", "Shipping initiated", etc.
        QuoteRevision       // Workshop requesting price/deadline change
    }
}

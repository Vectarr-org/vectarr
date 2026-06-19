using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class WorkOrderStatusEnumWrapper : BaseEnumClass<WorkOrderStatusEnum>
    {
   
        public override string GetDisplayText(WorkOrderStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case WorkOrderStatusEnum.CreatorAccepted:
                    return "Accepted";
                case WorkOrderStatusEnum.Active:
                    return "Active";
                case WorkOrderStatusEnum.Completed:
                    return "Completed";
                case WorkOrderStatusEnum.Cancelled:
                    return "Cancelled";
                case WorkOrderStatusEnum.Disputed:
                    return "Disputed";
                default:
                    return "";
            }
        }
    }
}

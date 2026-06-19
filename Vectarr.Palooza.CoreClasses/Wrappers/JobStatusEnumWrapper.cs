using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class JobStatusEnumWrapper : BaseEnumClass<JobStatusEnum>
    {
        public override string GetDisplayText(JobStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case JobStatusEnum.Draft:
                    return "Draft";
                case JobStatusEnum.InProgress:
                    return "In Progress";
                case JobStatusEnum.Completed:
                    return "Completed";
                case JobStatusEnum.Published:
                    return "Published";
                default:
                    return "";
            }
        }
    }
}

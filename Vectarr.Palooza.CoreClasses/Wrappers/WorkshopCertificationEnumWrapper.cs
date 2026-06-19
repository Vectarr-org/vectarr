using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class WorkshopCertificationEnumWrapper : BaseEnumClass<WorkshopCertificationEnum>
    {
        public override string GetDisplayText(WorkshopCertificationEnum enumValue)
        {
            switch (enumValue)
            {
                case WorkshopCertificationEnum.ISO9001:
                    return "ISO 9001";
                case WorkshopCertificationEnum.AS9100:
                    return "AS 9100";
                case WorkshopCertificationEnum.ISO13485:
                    return "ISO 13485";
                case WorkshopCertificationEnum.IATF16949:
                    return "IATF 16949";
                case WorkshopCertificationEnum.ISO14001:
                    return "ISO 14001";
                case WorkshopCertificationEnum.NADCAP:
                    return "NADCAP";
                case WorkshopCertificationEnum.Other:
                    return "Other";
                default:
                    return "";
            }
        }
    }
}

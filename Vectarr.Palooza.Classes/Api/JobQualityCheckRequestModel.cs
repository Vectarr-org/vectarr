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
    public class JobQualityCheckRequestModel : BaseApiModel
    {
        public QualityCheckTypeEnum CheckType { get; set; }
        public string? Instructions { get; set; }  // Optional custom instructions
        public int JobId { get; set; }
        public string CheckTypeText => new QualityCheckTypeEnumWrapper().GetDisplayText(CheckType);
    }
}

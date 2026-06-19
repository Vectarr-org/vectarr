using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkshopCustomProgressMessageModel : BaseApiModel
    {
        public string Text { get; set; }
        public int WorkShopId { get; set; }
    }
}

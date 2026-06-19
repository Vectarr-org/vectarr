using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class CreatorModel : BaseApiModel
    {
        public string? Bio { get; set; }
        public string? Company { get; set; }
        public AddressModel? Address { get; set; }
        public ImageModel? ProfilePicture { get; set; }
    }
}

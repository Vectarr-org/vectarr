using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.BaseApi
{
    public class BaseApiModel : IApiModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }

        public string ISOUTCCreatedDate => DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ");
        public string ISOUTCLastModified => LastModified.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }
}

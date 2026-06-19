using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.Api
{
    public class GenerateQuoteAuthModel
    {
        public string? Email { get; set; }
        public JobModel Job { get; set; }
        public string? Password { get; set; }

    }
}

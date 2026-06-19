using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Digifabster.Classes;

namespace Vectarr.Palooza.Digifabster
{
    public interface IDigifabsterService
    {
        Task<DigifabsterResult<RegisterClientResponse>> RegisterClientAsync(
            RegisterClientRequest request,
            CancellationToken cancellationToken = default);
    }
}

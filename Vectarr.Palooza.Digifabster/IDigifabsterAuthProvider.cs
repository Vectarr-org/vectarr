using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Digifabster
{
    public interface IDigifabsterAuthProvider
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}

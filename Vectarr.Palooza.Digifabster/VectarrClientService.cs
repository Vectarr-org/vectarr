using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Digifabster.Classes;

namespace Vectarr.Palooza.Digifabster
{
    public class VectarrClientService
    {
        private readonly IDigifabsterService _digifabster;

        public VectarrClientService(IDigifabsterService digifabster)
        {
            _digifabster = digifabster;
        }

        public async Task RegisterAsync(string email, string password)
        {
            var result = await _digifabster.RegisterClientAsync(new RegisterClientRequest
            {
                Email = email,
                Password = password,
                SilentLoginEnabled = true
            });

            if (!result.IsSuccess)
                throw new Exception($"Digifabster registration failed: {result.ErrorMessage}");
        }
    }
}

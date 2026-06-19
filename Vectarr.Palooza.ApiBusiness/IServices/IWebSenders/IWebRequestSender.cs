using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.ApiBusiness.IServices.IWebSenders
{
    public interface IWebRequestSender
    {
        Task<T> SendRequestAsync<T>(HttpMethod method, string url, HttpContent content = null, string token = null);
        Task<string> SendRequestAsync(HttpMethod method, string url, HttpContent content = null, string token = null);
    }
}

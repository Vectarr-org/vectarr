using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Exceptions
{
    public class DigifabsterApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? ResponseContent { get; }

        public DigifabsterApiException(string message) : base(message) { }

        public DigifabsterApiException(string message, HttpStatusCode statusCode, string? responseContent)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        public DigifabsterApiException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

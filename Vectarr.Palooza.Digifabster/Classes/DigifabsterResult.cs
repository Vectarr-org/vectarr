using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Digifabster.Classes
{
    public class DigifabsterResult<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? ErrorMessage { get; init; }
        public string? RawResponse { get; init; }

        public static DigifabsterResult<T> Success(T data, HttpStatusCode statusCode, string? rawResponse = null) => new()
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode,
            RawResponse = rawResponse
        };

        public static DigifabsterResult<T> Failure(HttpStatusCode statusCode, string errorMessage, string? rawResponse = null) => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            RawResponse = rawResponse
        };
    }
}

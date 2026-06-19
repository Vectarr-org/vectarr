using System.Net.Http;
using System.Text;
using System.Text.Json;
using Vectarr.Palooza.ApiBusiness.IServices.IWebSenders;

namespace Vectarr.Palooza.ApiBusiness.Services.WebSenders
{
    public class WebRequestSender : IWebRequestSender, IDisposable
    {
        private readonly HttpClient _httpClient;

        public WebRequestSender()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> SendRequestAsync(HttpMethod method, string url, HttpContent content = null, string token = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);

                if (content != null)
                {
                    request.Content = content;
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("Authorization", $"Bearer {token}");
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }

        public async Task<T> SendRequestAsync<T>(HttpMethod method, string url, HttpContent content = null, string token = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);

                if (content != null)
                {
                    request.Content = content;
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("Authorization", $"Bearer {token}");
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseBody);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return default;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vectarr.Palooza.Digifabster.Classes;

namespace Vectarr.Palooza.Digifabster
{
    public class DigifabsterService : IDigifabsterService
    {
        private readonly HttpClient _httpClient;
        private readonly IDigifabsterAuthProvider _authProvider;
        private readonly ILogger<DigifabsterService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        private const string ClientsEndpoint = "/v2/clients/";

        public DigifabsterService(
            HttpClient httpClient,
            IDigifabsterAuthProvider authProvider,
            ILogger<DigifabsterService> logger)
        {
            _httpClient = httpClient;
            _authProvider = authProvider;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
        }

        public async Task<DigifabsterResult<RegisterClientResponse>> RegisterClientAsync(
            RegisterClientRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Registering new client in Digifabster: {Email}", request.Email);

            var token = await _authProvider.GetTokenAsync(cancellationToken);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, ClientsEndpoint)
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reach Digifabster clients endpoint");
                return DigifabsterResult<RegisterClientResponse>.Failure(
                    System.Net.HttpStatusCode.ServiceUnavailable,
                    "Failed to reach Digifabster API.",
                    ex.Message);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Digifabster client registration failed with status {StatusCode}: {Response}",
                    response.StatusCode,
                    responseContent);

                return DigifabsterResult<RegisterClientResponse>.Failure(
                    response.StatusCode,
                    $"Client registration failed. Status: {response.StatusCode}",
                    responseContent);
            }

            var clientResponse = JsonSerializer.Deserialize<RegisterClientResponse>(responseContent, _jsonOptions);

            if (clientResponse is null)
            {
                _logger.LogWarning("Digifabster returned null response for client registration");
                return DigifabsterResult<RegisterClientResponse>.Failure(
                    response.StatusCode,
                    "Failed to deserialize Digifabster response.",
                    responseContent);
            }

            _logger.LogInformation("Successfully registered client in Digifabster: {Email}", request.Email);

            return DigifabsterResult<RegisterClientResponse>.Success(
                clientResponse,
                response.StatusCode,
                responseContent);
        }
    }

}

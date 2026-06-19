using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vectarr.Palooza.Digifabster.Classes;
using Vectarr.Palooza.Exceptions;

namespace Vectarr.Palooza.Digifabster
{
    public class DigifabsterAuthProvider : IDigifabsterAuthProvider
    {
        private readonly HttpClient _httpClient;
        private readonly DigifabsterOptions _options;
        private readonly ILogger<DigifabsterAuthProvider> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        private const string TokenEndpoint = "/v2/obtain_auth_token/";

        public DigifabsterAuthProvider(
            HttpClient httpClient,
            IOptions<DigifabsterOptions> options,
            ILogger<DigifabsterAuthProvider> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Obtaining new Digifabster auth token");

            var request = new ObtainAuthTokenRequest
            {
                CompanyName = _options.CompanyName,
                Email = _options.Email,
                Password = _options.Password
            };

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync(TokenEndpoint, request, _jsonOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reach Digifabster auth endpoint");
                throw new DigifabsterApiException("Failed to reach Digifabster auth endpoint.", ex);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Digifabster auth failed with status {StatusCode}: {Response}",
                    response.StatusCode,
                    responseContent);

                throw new DigifabsterApiException(
                    $"Failed to obtain auth token. Status: {response.StatusCode}",
                    response.StatusCode,
                    responseContent);
            }

            var tokenResponse = JsonSerializer.Deserialize<ObtainAuthTokenResponse>(responseContent, _jsonOptions);

            if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.Token))
            {
                _logger.LogError("Digifabster returned empty token. Response: {Response}", responseContent);
                throw new DigifabsterApiException("Digifabster returned an empty auth token.");
            }

            _logger.LogDebug("Successfully obtained Digifabster auth token");
            return tokenResponse.Token;
        }
    }
}
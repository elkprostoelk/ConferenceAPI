using ConferenceAPI.Core.Interfaces;
using ConferenceAPI.Core.ResponseModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ConferenceAPI.Core.Services
{
    public class ZoomAuthService : IZoomAuthService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ZoomAuthService> _logger;
        private readonly HttpClient _zoomAuthClient;
        private const string ZoomApiTokenCacheKey = "zoom_api_token";

        public ZoomAuthService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            ILogger<ZoomAuthService> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
            _zoomAuthClient = httpClientFactory.CreateClient("ZoomAuth");
        }

        public async Task<ZoomApiAuthTokenResponse?> GetZoomApiAccessTokenAsync()
        {
            var tokenExists = _memoryCache.TryGetValue<ZoomApiAuthTokenResponse>(ZoomApiTokenCacheKey, out var token);
            if (tokenExists && token is not null)
            {
                return token;
            }

            var accountId = _configuration["ZoomApp:AccountId"];
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            var tokenUri = QueryHelpers.AddQueryString(
                "token",
                new Dictionary<string, string?>
                {
                    { "grant_type", "account_credentials" },
                    { "account_id", accountId }
                });
            var result = await _zoomAuthClient.PostAsync(tokenUri, null);
            if (result.IsSuccessStatusCode)
            {
                var tokenResponse = await result.Content.ReadFromJsonAsync<ZoomApiAuthTokenResponse>();
                if (tokenResponse is null)
                {
                    throw new ArgumentNullException(nameof(tokenResponse));
                }

                return _memoryCache.Set(ZoomApiTokenCacheKey, tokenResponse, TimeSpan.FromHours(1));
            }

            _logger.LogError("Zoom API auth request failed! Status code: {StatusCode}", result.StatusCode);
            return null;
        }
    }
}

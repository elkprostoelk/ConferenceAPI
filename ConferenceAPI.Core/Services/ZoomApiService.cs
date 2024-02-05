using System.Net.Http.Json;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Core.Interfaces;
using ConferenceAPI.Core.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConferenceAPI.Core.Services
{
    public class ZoomApiService : IZoomApiService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ZoomApiService> _logger;
        private readonly HttpClient _zoomApiClient;
        private readonly HttpClient _zoomAuthClient;

        private const string ZoomApiTokenCacheKey = "zoom_api_token";

        public ZoomApiService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            ILogger<ZoomApiService> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
            _zoomApiClient = httpClientFactory.CreateClient("ZoomAPI");
            _zoomAuthClient = httpClientFactory.CreateClient("ZoomAuth");
        }

        public async Task<ZoomMeeting?> CreateZoomMeeting()
        {
            var tokenResponse = await GetZoomApiAccessTokenAsync();
            if (tokenResponse is null)
            {
                return null;
            }

            _zoomApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var requestBody = new CreateZoomMeetingDto
            {
                Topic = "My Zoom Meeting",
                Type = 2,
                StartTime = DateTime.Now,
                Duration = 90
            };

            var response = await _zoomApiClient.PostAsJsonAsync("users/me/meetings", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ZoomMeeting>();
                return result;
            }
            else
            {
                return null;
            }
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

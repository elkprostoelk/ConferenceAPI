using System.Net.Http.Json;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Core.Interfaces;
using ConferenceAPI.Core.ResponseModels;
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

        public async Task<ZoomMeetingDto?> CreateZoomMeeting(string email, CreateZoomMeetingDto createZoomMeetingDto)
        {
            var tokenResponse = await GetZoomApiAccessTokenAsync();
            if (tokenResponse is null)
            {
                return null;
            }

            _zoomApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _zoomApiClient.PostAsJsonAsync($"users/{email}/meetings", createZoomMeetingDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ZoomMeetingDto>();
                return result;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("A Zoom user {UserEmail} has not been found!", email);
                }

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

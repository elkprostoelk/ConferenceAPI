using System.Net;
using System.Net.Http.Json;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ConferenceAPI.Core.Services
{
    public class ZoomApiService : IZoomApiService
    {
        private readonly IZoomAuthService _zoomAuthService;
        private readonly ILogger<ZoomApiService> _logger;
        private readonly HttpClient _zoomApiClient;

        public ZoomApiService(
            IZoomAuthService zoomAuthService,
            IHttpClientFactory httpClientFactory,
            ILogger<ZoomApiService> logger)
        {
            _zoomAuthService = zoomAuthService;
            _logger = logger;
            _zoomApiClient = httpClientFactory.CreateClient("ZoomAPI");
        }

        public async Task<ZoomMeetingDto?> CreateZoomMeeting(string email, CreateZoomMeetingDto createZoomMeetingDto)
        {
            var tokenResponse = await _zoomAuthService.GetZoomApiAccessTokenAsync();
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
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("A Zoom user {UserEmail} has not been found!", email);
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("A number of 100 requests for a user per day has been exceeded!");
                }

                return null;
            }
        }

        public async Task<bool> DeleteZoomMeetingAsync(long id)
        {
            var tokenResponse = await _zoomAuthService.GetZoomApiAccessTokenAsync();
            if (tokenResponse is null)
            {
                return false;
            }

            _zoomApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _zoomApiClient.DeleteAsync($"meetings/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                _logger.LogWarning(
                    "A request Delete Meeting {Id} has returned {StatusCode}. Response body: {ResponseBody}",
                    id,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());
                
                return false;
            }
        }

        public async Task<ZoomMeetingDto?> GetZoomMeetingByIdAsync(long id)
        {
            var tokenResponse = await _zoomAuthService.GetZoomApiAccessTokenAsync();
            if (tokenResponse is null)
            {
                return null;
            }

            _zoomApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _zoomApiClient.GetAsync($"meetings/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ZoomMeetingDto>();
                return result;
            }
            else
            {
                _logger.LogWarning(
                    "A request Get Meeting {Id} has returned {StatusCode}. Response body: {ResponseBody}",
                    id,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());

                return null;
            }
        }

        public async Task<ZoomMeetingStatisticsDto?> GetZoomMeetingStatisticsAsync(long id)
        {
            var tokenResponse = await _zoomAuthService.GetZoomApiAccessTokenAsync();
            if (tokenResponse is null)
            {
                return null;
            }

            _zoomApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var requestUri = QueryHelpers.AddQueryString(
                $"metrics/meetings/{id}/participants",
                new Dictionary<string, string?>
                {
                    { "type", "past" },
                    { "page_size", "200" }
                });
            var response = await _zoomApiClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ZoomMeetingStatisticsDto>();
            }
            else
            {
                _logger.LogWarning(
                    "A request Get Meeting Statistics {Id} has returned {StatusCode}. Response body: {ResponseBody}",
                    id,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());

                return null;
            }
        }
    }
}

﻿using System.Net;
using System.Net.Http.Json;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Core.Interfaces;
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
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("A request Get Meeting {MeetingId} has returned 404. Error: {Error}", id, await response.Content.ReadAsStringAsync());
                }

                return null;
            }
        }
    }
}

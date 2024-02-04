using System.Net.Http.Json;
using System.Text.Json;
using ConferenceAPI.Core.Models;

namespace ConferenceAPI.Core.Services
{
    public class ZoomApiService : IZoomApiService
    {
        public async Task<string> CreateZoomMeeting(string accessToken)
        {
            string apiUrl = "https://api.zoom.us/v2/";
            using HttpClient client = new();
            string createMeetingEndpoint = $"{apiUrl}users/me/meetings";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var requestBody = new
            {
                topic = "My Zoom Meeting",
                type = 2
            };

            var response = await client.PostAsJsonAsync(createMeetingEndpoint, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ZoomMeetingResponse>(content);
                return result.id.ToString();
            }
            else
            {
                return null;
            }
        }

        
    }
}

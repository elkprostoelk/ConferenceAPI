using System.Text.Json.Serialization;

namespace ConferenceAPI.Core.Models
{
    public class ZoomApiAuthTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}

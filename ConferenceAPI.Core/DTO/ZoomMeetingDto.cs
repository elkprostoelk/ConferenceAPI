using ConferenceAPI.Core.Enums;
using System.Text.Json.Serialization;

namespace ConferenceAPI.Core.DTO
{
    public class ZoomMeetingDto
    {
        public long Id { get; set; }

        [JsonPropertyName("join_url")]
        public string JoinUrl { get; set; }

        [JsonPropertyName("start_url")]
        public string StartUrl { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonPropertyName("host_email")]
        public string HostEmail { get; set; }

        public MeetingType Type { get; set; }

        public string Password { get; set; }

        public CreateZoomMeetingSettingsDto? Settings { get; set; }
    }
}

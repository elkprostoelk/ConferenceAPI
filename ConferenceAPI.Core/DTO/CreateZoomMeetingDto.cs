using System.Text.Json.Serialization;

namespace ConferenceAPI.Core.DTO
{
    public class CreateZoomMeetingDto
    {
        public string Topic { get; set; }

        public int Type { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        public int Duration { get; set; }
    }
}

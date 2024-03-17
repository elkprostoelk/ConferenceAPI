using ConferenceAPI.Core.Enums;
using System.Text.Json.Serialization;

namespace ConferenceAPI.Web.Models
{
    public class CreateZoomMeetingModel
    {
        public required string Topic { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        public int Duration { get; set; }
        
        public MeetingType Type { get; set; }

        public CreateZoomMeetingSettingsModel? Settings { get; set; }

        public required string Email { get; set; }
    }

    public class CreateZoomMeetingSettingsModel
    {
        [JsonPropertyName("waiting_room")]
        public bool? WaitingRoom { get; set; }

        [JsonPropertyName("join_before_host")]
        public bool? JoinBeforeHost { get; set; }
    }
}

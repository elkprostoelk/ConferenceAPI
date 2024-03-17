using ConferenceAPI.Core.Enums;
using System.Text.Json.Serialization;

namespace ConferenceAPI.Core.DTO
{
    public class CreateZoomMeetingDto
    {
        public required string Topic { get; set; }

        public MeetingType Type { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        public CreateZoomMeetingSettingsDto? Settings { get; set; }

        public int Duration { get; set; }

        public string Password { get; set; }
    }

    public class CreateZoomMeetingSettingsDto
    {
        [JsonPropertyName("waiting_room")]
        public bool? WaitingRoom { get; set; }

        [JsonPropertyName("join_before_host")]
        public bool? JoinBeforeHost { get; set; }

        [JsonPropertyName("mute_upon_entry")]
        public bool MuteUponEntry { get; set; } = true;
    }
}

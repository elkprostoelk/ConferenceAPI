using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConferenceAPI.Core.DTO
{
    public class ZoomMeetingStatisticsDto
    {
        public List<ZoomMeetingParticipantDto> Participants { get; set; }

        [JsonPropertyName("total_records")]
        public int TotalRecords { get; set; }
    }

    public class ZoomMeetingParticipantDto
    {
        public string Email { get; set; }

        [JsonPropertyName("join_time")]
        public DateTime JoinTime { get; set; }

        [JsonPropertyName("leave_time")]
        public DateTime LeaveTime { get; set; }

        [JsonPropertyName("leave_reason")]
        public string LeaveReason { get; set; }
    }
}

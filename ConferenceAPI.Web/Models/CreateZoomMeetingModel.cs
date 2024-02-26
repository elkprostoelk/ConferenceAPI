namespace ConferenceAPI.Web.Models
{
    public class CreateZoomMeetingModel
    {
        public required string Topic { get; set; }

        public DateTime StartTime { get; set; }

        public int Duration { get; set; }

        public int Type { get; set; }

        public required string Email { get; set; }
    }
}

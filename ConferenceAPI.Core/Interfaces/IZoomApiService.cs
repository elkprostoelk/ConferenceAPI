using ConferenceAPI.Core.DTO;

namespace ConferenceAPI.Core.Interfaces
{
    public interface IZoomApiService
    {
        Task<ZoomMeetingDto?> CreateZoomMeeting(string email, CreateZoomMeetingDto createZoomMeetingDto);
        Task<bool> DeleteZoomMeetingAsync(long id);
        Task<ZoomMeetingDto?> GetZoomMeetingByIdAsync(long id);
        Task<ZoomMeetingStatisticsDto?> GetZoomMeetingStatisticsAsync(long id);
    }
}

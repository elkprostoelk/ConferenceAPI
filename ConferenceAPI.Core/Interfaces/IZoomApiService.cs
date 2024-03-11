using ConferenceAPI.Core.DTO;
using ConferenceAPI.Core.ResponseModels;

namespace ConferenceAPI.Core.Interfaces
{
    public interface IZoomApiService
    {
        Task<ZoomMeetingDto?> CreateZoomMeeting(string email, CreateZoomMeetingDto createZoomMeetingDto);
        Task<bool> DeleteZoomMeetingAsync(long id);
        Task<ZoomMeetingDto?> GetZoomMeetingByIdAsync(long id);
    }
}

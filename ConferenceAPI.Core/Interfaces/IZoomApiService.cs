using ConferenceAPI.Core.Models;

namespace ConferenceAPI.Core.Interfaces
{
    public interface IZoomApiService
    {
        Task<ZoomMeeting?> CreateZoomMeeting();
        Task<ZoomApiAuthTokenResponse?> GetZoomApiAccessTokenAsync();
    }
}

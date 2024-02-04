namespace ConferenceAPI.Core.Services
{
    public interface IZoomApiService
    {
        Task<string> CreateZoomMeeting(string accessToken);
    }
}

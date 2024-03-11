using ConferenceAPI.Core.ResponseModels;

namespace ConferenceAPI.Core.Interfaces
{
    public interface IZoomAuthService
    {
        Task<ZoomApiAuthTokenResponse?> GetZoomApiAccessTokenAsync();
    }
}

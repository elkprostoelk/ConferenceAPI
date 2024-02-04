using ConferenceAPI.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceAPI.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoomApiController : ControllerBase
    {
        public readonly ZoomApiService _zoomApiService = new ZoomApiService();

        [HttpGet("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMeeting([FromQuery]string accessToken)
        {
            string result = await _zoomApiService.CreateZoomMeeting(accessToken);
            return (result == null) ? BadRequest() : Ok(result);
        }
    }
}

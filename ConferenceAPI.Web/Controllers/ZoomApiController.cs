using ConferenceAPI.Core.Interfaces;
using ConferenceAPI.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceAPI.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoomApiController : ControllerBase
    {
        public readonly IZoomApiService _zoomApiService;

        public ZoomApiController(
            IZoomApiService zoomApiService)
        {
            _zoomApiService = zoomApiService;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(ZoomMeeting), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMeeting()
        {
            var meeting = await _zoomApiService.CreateZoomMeeting();
            return meeting is not null
                ? CreatedAtAction(nameof(CreateMeeting), meeting)
                : BadRequest();
        }
    }
}

using AutoMapper;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Core.Interfaces;
using ConferenceAPI.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceAPI.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoomApiController : ControllerBase
    {
        private readonly IZoomAuthService _zoomAuthService;
        public readonly IZoomApiService _zoomApiService;
        private readonly IMapper _mapper;

        public ZoomApiController(
            IZoomAuthService zoomAuthService,
            IZoomApiService zoomApiService,
            IMapper mapper)
        {
            _zoomAuthService = zoomAuthService;
            _zoomApiService = zoomApiService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(ZoomMeetingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateMeeting(CreateZoomMeetingModel createZoomMeetingModel)
        {
            var token = await _zoomAuthService.GetZoomApiAccessTokenAsync();
            if (token is null)
            {
                return Unauthorized();
            }

            var createZoomMeetingDto = _mapper.Map<CreateZoomMeetingDto>(createZoomMeetingModel);

            var meeting = await _zoomApiService.CreateZoomMeeting(createZoomMeetingModel.Email, createZoomMeetingDto);
            return meeting is not null
                ? CreatedAtAction(nameof(CreateMeeting), meeting)
                : BadRequest();
        }

        [HttpGet("meeting/{id:long}")]
        [ProducesResponseType(typeof(ZoomMeetingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetZoomMeetingById(long id)
        {
            var token = await _zoomAuthService.GetZoomApiAccessTokenAsync();
            if (token is null)
            {
                return Unauthorized();
            }

            var meeting = await _zoomApiService.GetZoomMeetingByIdAsync(id);

            return meeting is not null
                ? Ok(meeting)
                : BadRequest();
        }
    }
}

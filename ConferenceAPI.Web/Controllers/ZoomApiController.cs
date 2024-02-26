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
        public readonly IZoomApiService _zoomApiService;
        private readonly IMapper _mapper;

        public ZoomApiController(
            IZoomApiService zoomApiService,
            IMapper mapper)
        {
            _zoomApiService = zoomApiService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(ZoomMeetingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMeeting(CreateZoomMeetingModel createZoomMeetingModel)
        {
            var createZoomMeetingDto = _mapper.Map<CreateZoomMeetingDto>(createZoomMeetingModel);

            var meeting = await _zoomApiService.CreateZoomMeeting(createZoomMeetingModel.Email, createZoomMeetingDto);
            return meeting is not null
                ? CreatedAtAction(nameof(CreateMeeting), meeting)
                : BadRequest();
        }
    }
}

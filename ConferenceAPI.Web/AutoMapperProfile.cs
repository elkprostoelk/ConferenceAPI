using AutoMapper;
using ConferenceAPI.Core.DTO;
using ConferenceAPI.Web.Models;

namespace ConferenceAPI.Web
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateZoomMeetingModel, CreateZoomMeetingDto>();
        }
    }
}
